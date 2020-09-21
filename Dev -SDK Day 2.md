# TB1300 - SDK (SAP Development Kit)

## 개요

### 2 Tier

Fat Client 방식

#### Windows NT 계열 

SAP 에서 SDK에 포함시켰다. 

UI API, DI API 가 기본이 되는 개념 COM, DLL로 되어있다.

SDK= DLL+COM 모델로 컴퓨터에 등록, 다른 컴퓨터와 통신할 수 있는 모델 

-> Ditstributed Component of Model 은 .NET으로 하는 개념 확장된 것이다.

- COM : component Object Model
- DLL : Dynamic link library



#### COM과 DLL을 이용해서 만든 것.

- UI API : 스크린 화면에 컨트롤 제어 하는 것
- DI API : 데이터를 핸들링 하기 위한 프로그래밍 인터페이스
- windows에 종속적임. 
- 클라이언트에 설치됨.



### Service Layer (DI Server)

- DI Server 보다 진보된 것으로, 같은 개념이라 생각해도 무방!

- HTTPs/OData  (e.g. JSON, Atom)

- REST 방식

- windows에 종속적인 UI, DI를 거치지 않고 서버단에서 되기 때문에 Device 독립적임.

- 비즈니스 로직이 진행됨.

- Request / Response 방식

- Any Device (Mobile / Web)

- 순차적이 아니라 병렬처리하여 퍼포먼스 상승

- 로드 밸런싱

- DI는 soap 인터페이스 식으로 html, xml,, 등의 형태가 된다. 

- 데이터를 활용해 동시에 서비스를 접근할 수 있도록 한다. 

  

#### ODBC

MS에서 만든 데이터 핸들 위한 DB접근



##### ODATA (OPEN + DATA)

조회, 등록, 수정, 삭제 등을 유연하게 관리할 수 있게 한다.



#### SCP (SAP Cloud Platform)

- PAAS
- SAAS
- IAAS

[sap community network](https://community.sap.com/)



# DI API

Doc과 어떤 Property가 연결되어 있는지 아는 게 중요하다.

PC에 깔려서 직접적으로 B1 HANA DB와 연결된다.

메소드, 속성 등을 노출시켜두는 것 Observer DLL이 핵심이 된다.



### 3 Objects Categories **기억!

Business와 Non-Business

- **Business Obj**  구매오더에 해당하는 오브젝트 (제일 많이 사용 -> 중요)
  
  - Master Data
  - Transactional Data Obj
    - JE
    - Document
  
  => Business Obj : 구성되어 있는 모든 데이터 관련된 핸들을 메소드 형태로 노출해 둔다. 



- Infra Obj
  - **Company Obj** (최상위 객체로 많이 사용, HANA랑 SAP B1이랑 연결 할 수 있는 첫 번째)

    - company obj가 무엇? -> DB의 경우 insert, update... 할때 트랜잭션 관리를 한다. 화면을 저장하면 특	정비즈니스를 처리해줘야 한다. 만약 TB1 데이터 insert 동시에 TB2 연관된 Foreign key를 update 해	줘야 하는 니즈가 있을 때 TB1에는 insert 하는 구문이 실행되고, TB2에 특정 로우 데이터가 update된	다. 그런데, 둘을 동시에 할 때 TB1은 성공, TB2는 실패하면 롤백해야 한다. 따라서 위에 있는 트랜잭션	을 지운다. 그게 Company Obj!
    - 출하를 등록 했다. -> 판매 오더 입장에서 판매 오더 라인에 출하로 전기가 된 수량을 등록한다. -> 데이터 생성, 출하요청 리퀘스트 수량에 맞게 SBO에서 출하요청 1, 판매오더 2 두객체를 호출 -> 호출하기 전 Company Obj에서 start object를 통해 제어할 수 있다.
    - All or Not 원칙을 이것을 이용해서 제어 가능.

  - Ex Functionality Obj (확장 함수 오브젝트로 많이 사용)
    - RecordSet
    - DataBrowser
    - SBObob
    
  - Meta Data Obj : 테이블 명세서

    데이터를 저장해서 관리하는 것이 메타 데이터다. 즉, 정의서를 위한 정의내역

  
- Special Obj ( UI와 연결되어 있는 개체)
  - Service Type 환경설정
    
    - *CompanyService*
  - Defined Obj related  to SBO GUI
    
    - ChooseFromList : 객체 팝업 도우미
    
    



# UI API

DI API 보다는 UI API에 집중

-s로 끝나면 배열이라는 뜻이다. 

### Scope

### Characteristic

- Multi 3rd party Add on 처리



### Object and Collections

#### Application

 최상위 객체

#### DataSources

Data의 attribute를 가지고 있음.

- DBDataSource
- UserDataSource
- DataTable



### Add-ons

#### Connection String

애드온의 쿠키 값 이 값으로 애드온을 판별하고 실행한다.



#### Single Signed on

한 번의 인증(로그인)으로 나머지 쓸 수 있음. 여기서 나머지란, DI, 애드온 등를 말함.

UI API 와 DI API 가 결합하기에 DI API 에서 유니크한 문자를 만들어서 UI에 전달하고 DB에 보내야 하는데, 별도의 DB 커넥션 없이 연동해서 UI API와 결합하는 것이다.



#### Multi Add-ons using DI API 

기존에 있던 것을 사용



### WMS??

빈 레벨 Bean 레벨?

### Batch

실시간이 아닌 주기적으로 돌아가는 프로그램.



# 샘플 실습

샘플소스는 C:\Program Files (x86)\SAP\SAP Business One SDK\Samples 에 위치



### Module

static 객체



### 프로젝트

모든 파일 보기 후 참조- 깨진 파일은 삭제하고, 참조 추가해서 UI 10.0으로 선택 후 빌드한다.

- Hello World
- Catching Events
  
  - 어떤 이벤트가 있는지 / Before, After
  
  

### Event

- Before : 화면을 그려주고 데이터를 링킹한다.
- After : valid 체크같은 기능이 수행된다. (e.g. keypressed 또는 focusing)
- 로그온하면 UI API를 통해 client와 애드온이 Event Sync하고 Hook method로 대기하고 있다가 이벤트를 Hook한다.



### 디버깅

- BreakPoint
- F5 다음 브레이크 포인트
- F10 프로시저 단위
- F11 한 줄 단위
- 조사식 이용 변수 값 보기



### Help 

UI - HowTo - 필요한 거 볼 것.

DI - HowTo - 1번. DB 연결



SAP Business One SDK  - help center 참조