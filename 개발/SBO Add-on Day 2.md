# UI

----

## 1. EVENT

- Before : 버튼 클릭하고 떼기 전까지
- After : 떼고 난 후

### ITEM Event

- validate() : 값이 바뀌었을 때. 

### Menu Event

### Form Data Event

이벤트 처리 순서

1. 일반 이벤트(Before / After) 등
2. 폼 데이터 이벤트
3. 노티

> NOTI를 대체할 수 있음.
>
> 현재 문제 : 인풋체크 등의 문제를 이벤트에서 처리하지 않고 NOTI에서 쓰고 있어서 트랜잭션 과부하로 전체 시스템이 느려짐
>
> 이 이벤트를 써서 노티에 몰린 트랜잭션을 분산 시켜야 함.  하지만, 애드온이 뻗어버리면 데이터 정합성이 꺠짐. 노티는 DB단에서 막아버려서 애드온 뻗는 것과 상관없이 값 체크 가능.

### Right-Click Event



## 2. SCREEN Painter 실습

### 컨트롤

### 네이밍룰

- #### 오브젝트 차원



## 3, 애드온 개발 실습

WJS와 wjs_site 2개 프로젝트로 구성된다.

전자는 응용프로그램 후자는 클래스 라이브리이며, Dll파일을 가지고 있다.



어셈블리 이름 : exe파일 이름이 됨.

xml_int : DBSetting Client = "'----;'" 만 사용됨.



> 관리자 모드에서 실행할 것. MSSQL 보안 사용자에서 public 외에, sysadmim도 체크할 것

> 폼하나 추가하기 위해서 srf파일 추가과 vb파일 추가, xml파일에서 추가해주어야 하고, UID 및 father ID, 모듈 아이디를 맞춰주어야 한다.