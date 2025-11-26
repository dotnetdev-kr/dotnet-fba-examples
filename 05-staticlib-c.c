#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <iconv.h>

// Compile with:
// gcc -o 05-staticlib-c 05-staticlib-c.c artifacts/05-native-aot/05-native-aot.dylib -Wl,-rpath,artifacts/05-native-aot -liconv

// Run with:
// ./05-staticlib-c

// .NET Native AOT에서 생성된 함수 선언
extern void mymethod(void*);

// UTF-8을 UTF-16 LE로 변환하여 mymethod 호출
void call_mymethod(const char*);

int main(int argc, const char** argv) {
    printf("Testing Native AOT static library from C\n");
    printf("==========================================\n\n");
    
    call_mymethod("Hello from C!");
    call_mymethod("Native AOT 호출 테스트");
    call_mymethod("안녕하세요!");
    
    printf("\nAll calls completed.\n");
    
    return 0;
}

void call_mymethod(const char* text) {
    iconv_t cd = iconv_open("UTF-16LE", "UTF-8");
    if (cd == (iconv_t)-1) {
        perror("iconv_open");
        return;
    }
    
    size_t inbytes = strlen(text);
    size_t outbytes = (inbytes + 1) * 4; // 충분한 버퍼 크기
    
    char* inbuf = (char*)text;
    char* outbuf = (char*)malloc(outbytes);
    char* outptr = outbuf;
    
    if (outbuf == NULL) {
        iconv_close(cd);
        return;
    }
    
    if (iconv(cd, &inbuf, &inbytes, &outptr, &outbytes) == (size_t)-1) {
        perror("iconv");
        free(outbuf);
        iconv_close(cd);
        return;
    }
    
    // null terminator 추가
    *outptr++ = 0;
    *outptr = 0;
    
    mymethod((void*)outbuf);
    
    free(outbuf);
    iconv_close(cd);
}
