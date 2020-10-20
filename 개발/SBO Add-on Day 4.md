### 표준 폼 생성 절차

1. Site 프로젝트의 경우 WJS_init에 Site명이 기술되어 있어야 함.

2. SRF

   1. 스크린 페인터로 폼 생성
   2. UID 부여
   3. 프로젝트에 파일 추가

3. 데이터 생성

4. 메뉴 추가

   1. 경로에 XML파일 생성
   2. 메뉴 태그 추가
   3. FormUID, MenuUID, VB파일명

5. VB파일 생성

   1. 프로젝트에 파일 추가
   2. 데이터 바인딩
   3. 폼 디폴트
   4. ChooseFromList
   5. 이벤트 처리
   6. 질의 생성해서 로직 처리

   

### SRF 파일 정렬

Visual Studio - 도구 - 텍스트 편집기 - 확장자명 srf / xml 텍스트 편집기 

보기 - 도구모음 - xml 편집기 켜기 - 선택 영역 다시 지정



### 메뉴 태그 속성 

#### FatherUID property

시스템 정보 ON / 시스템 메뉴 - 모듈 - 하위 메뉴 UID

#### Type property

- type = 1 하위 메뉴 없음
- type = 2 하위 메뉴 있음, 트리구조





cfl.common_message("!","Hello")

### Linked Button

linkedObject : Doc ObjType

linkedTo : editText