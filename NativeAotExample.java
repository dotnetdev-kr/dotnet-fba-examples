// 컴파일:
// javac -cp jna-5.14.0.jar 05-native-aot.java

// 실행:
// java -cp .:jna-5.14.0.jar NativeAotExample

// JNA 다운로드:
// wget https://repo1.maven.org/maven2/net/java/dev/jna/jna/5.14.0/jna-5.14.0.jar

import com.sun.jna.Library;
import com.sun.jna.Native;
import com.sun.jna.Pointer;
import com.sun.jna.Memory;
import java.nio.charset.StandardCharsets;
import java.nio.file.Paths;

public class NativeAotExample {
    // .NET Native AOT 라이브러리 인터페이스
    public interface NativeAotLib extends Library {
        // artifacts/05-native-aot/05-native-aot.dylib 로드
        // 절대 경로 사용
        String LIBRARY_PATH = Paths.get(System.getProperty("user.dir"), 
            "artifacts", "05-native-aot", "05-native-aot.dylib").toString();
        NativeAotLib INSTANCE = Native.load(LIBRARY_PATH, NativeAotLib.class);
        
        // void mymethod(void* ptrText)
        void mymethod(Pointer ptrText);
    }
    
    // UTF-16 LE 문자열을 생성하여 mymethod 호출
    public static void callMyMethod(String text) {
        // UTF-16 LE 인코딩 + null terminator
        byte[] utf16Bytes = text.getBytes(StandardCharsets.UTF_16LE);
        
        // null terminator 추가를 위해 2바이트 더 할당
        Memory memory = new Memory(utf16Bytes.length + 2);
        memory.write(0, utf16Bytes, 0, utf16Bytes.length);
        memory.setByte(utf16Bytes.length, (byte) 0);
        memory.setByte(utf16Bytes.length + 1, (byte) 0);
        
        // mymethod 호출
        NativeAotLib.INSTANCE.mymethod(memory);
    }
    
    public static void main(String[] args) {
        System.out.println("Testing Native AOT library from Java");
        System.out.println("======================================\n");
        
        callMyMethod("Hello from Java!");
        callMyMethod("Native AOT 호출 테스트");
        callMyMethod("안녕하세요!");
        
        System.out.println("\nAll calls completed.");
    }
}
