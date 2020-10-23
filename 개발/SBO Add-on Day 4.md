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



### Before와 After Event

Before는 Sub가 아니고 Function 즉, 리턴값이 있음.

After는 Sub이다.



### SDK 복습

- DataBind
- SetBound
- 디폴트 값 세팅
- 콤보박스 값 쿼리로 세팅
- EditText에 FMS(FormattedSearch)  세팅
- 체크 박스 기본 값(2가지로 DataSource, UI)
- 라디오 버튼
- 라디오 버튼과 체크박스 이벤트 처리 BF/AF



### 프로시저

- 매개변수
  - = '값'으로 디폴트 값 주기 가능
- SET 설정 3가지
  1. SET NOCOUNT ON
  2. SET ANSI WARNING OFF
  3. SET ARITHIGNORE ON
- EXAC PROC_NAME 으로 실행 가능



### 그리드

- ChooseFromList
- 