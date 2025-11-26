#!/usr/bin/env node

// koffi 설치:
// npm install koffi

// 실행:
// node 05-native-aot.js

const koffi = require('koffi');
const path = require('path');

// 절대 경로로 dylib 로드
const libraryPath = path.join(__dirname, 'artifacts', '05-native-aot', '05-native-aot.dylib');

// .NET Native AOT 라이브러리 로드
const lib = koffi.load(libraryPath);

// void mymethod(void* ptrText)
const mymethod = lib.func('mymethod', 'void', ['void*']);

// UTF-16 LE 문자열을 생성하여 mymethod 호출
function callMyMethod(text) {
    // UTF-16 LE 인코딩 + null terminator
    const buffer = Buffer.from(text, 'utf16le');
    const nullTerminator = Buffer.alloc(2);
    const combined = Buffer.concat([buffer, nullTerminator]);
    
    // mymethod 호출
    mymethod(combined);
}

console.log('Testing Native AOT library from Node.js');
console.log('==========================================\n');

callMyMethod('Hello from Node.js!');
callMyMethod('Native AOT 호출 테스트');
callMyMethod('안녕하세요!');

console.log('\nAll calls completed.');
