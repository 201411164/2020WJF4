# SDK

## 개요

### 2 Tier

Fat Client 방식

#### Windows NT 계열 

COM : component Object Model

DLL : Dynamic link library

#### COM과 DLL을 이용해서 만든 것.

- UI API
- DI API
- windows에 종속적임.
- 클라이언트에 설치됨.

### Service Layer

- HTTPs/OData  (e.g. JSON, Atom)
- REST 방식
- windows에 종속적인 UI, DI를 거치지 않고 서버단에서 되기 때문에 Device 독립적임.
- 비즈니스 로직이 진행됨.

#### ODBC

MS에서 만든 데이터 핸들 위한 DB접근

#### SCP (SAP Cloud Platform)

- PAAS
- SAAS
- IAAS

[sap community network](https://community.sap.com/)

### DI API / Service Layer(DI Server)

- Request / Response 방식
- Any Device (Mobile / Web)
- 순차적이 아니라 병렬처리하여 퍼포먼스 상승
- 로드 밸런싱



# DI API

Doc과 어떤 Property가 연결되어 있는지 아는 게 중요하다.

### 3 Objects Categories

Business와 Non-Business

- **Business Obj** 제일 중요함.
  - Master Data
  - Transactional Data Obj
    - JE
    - Document
- Infra Obj
  - **Company Obj** (최상위 객체)
  - Ex Functionality Obj
    - RecordSet
    - DataBrowser
    - SBObob
  - Meta Data : 테이블 명세서
- Special Obj
  - Service Type 환경설정
    - *CompanyService*
  - Defined Obj related  to SBO GUI
    - ChooseFromList

### Company Object

- All or Not 원칙을 이것을 이용해서 제어 가능.



# UI API

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

### Single Signed on

한 번의 인증(로그인)으로 나머지 쓸 수 있음. 여기서 나머지란, DI, 애드온 등를 말함.



### WMS??

빈 레벨 Bean 레벨?

### Batch

실시간이 아닌 주기적으로 돌아가는 프로그램.



# 샘플 실습

### Module

스태틱 객체

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