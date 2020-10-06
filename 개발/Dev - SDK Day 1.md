# SDK  9.3 implement 13 14 15

## 프로세스 정리

Capacity + Cost = 원가

SDK

Integration framework 대신 B1F.

Service Layer : 기존 설치형 클라이언트 서비스가 아니라 server단, DI서버에서 발전, 분산 처리에 강함.

관리회계 / 손익회계 = 원재료의 원가부터 내정되어 있는 제품의 표준원가까지 판단해 이 제품의 원가 가치가 낮은지 / 높은지 판단하는 것!



### 마스터 데이터

- BP

- MM

- 품목 그룹

  - 상품

  - 제품

    반제품(중간재)
    - 원재료 : 수량 관리, 물리적으로 카운팅 (예 : 박스, 개)
    - 원료 : 단위로 관리, 양적으로 카운팅 ( 예 : 액체 )

- GL 계정과목.

  - 조정계정

     : 14일 단가 10원 -> 17일 단가 9원으로 변했다. 이때 조정계정으로 조절

  - wip 차이 계정

- 가격 리스트

- 리드 타임 : 생산마감시간이 있을때, 어느시점에 투입할지 정하는 것. 

  각 공정마다 원단, 실 .. 등을 투입해야 할것 이다. 마지막 공정에 단가가 낮은 소모품 재고를 투입하는 것을 판단하는 것이 예가 될 수 있다.

### 생산

감가상각

내용 연수 : 감가상각이 적용되는 기간의 연수

EX) 노트북을 100만원에 취득해 4년 뒤 20만원의 잔존가치까지 정액법으로 감가상각을 계산한다 가정하면

1년 뒤에 감가상각비는 (100-20)X(1/4)=80만원, 2년 뒤에 60만원, 4년뒤에 20만원

이 경우에 내용 연수는 4년이다.



생산에 소비된 재료의 분개

이동평균으로 금액 재료비 또는 WIP (Work In Process) 

* WIP : 생산관리 과정에서 원단, 염색약 ... 등 원재료를 대차변에 나타낼때 사용한다. 가계정과 비슷한 개념

  WIP 차이 계정 - 차, 대변에 위치한 WIP 값의 차이만큼 조정하기 위한 계정



| 생산 | 차                                       | 대           |
| ---- | ---------------------------------------- | :----------- |
| 오더 | WIP / 재료비 (이동 평균 금액)            | 원재료       |
| 입고 | 제품                                     | 직 간접 비용 |
|      | 제조원가(노무비, 재료비, 경비 ) 표준원가 | WIP          |

원가 = 제조원가 + 영업비용(= 판매비 + 관리비)



# 질의

UI API, DI API 를 이용해서  DB추가, TABLE 추가 하므로써 코딩없이 사용자 API를 구성하는 방법을 코어에서 제공해 준다. 이 과정에서 질의 사용한다. 



### 커스터마이징 툴

[뷰 - 시스템정보 활성화] : 필드의 테이블 이름 알 수 있다. 



### 질의 툴

#### 질의 생성기의 질의요소 - WHERE절

##### 조회

마감("C") 구매오더 조회

```mssql
SELECT T0.[DocNum], T0.[CardCode], T0.[CardName], T0.[DocTotal] 
FROM OPOR T0 WHERE T0.[DocStatus]  = 'C' 
ORDER BY T0.[DocDate]
```



전기일 값을 현재일자에서 -7 계산 조회

```mssql
SELECT T0.[CardCode], T0.[CardName], T0.[DocTotal], T0.[DocNum] 
FROM OPOR T0 WHERE T0.[DocStatus]  = 'C' 
and T0.[DocDate] > ADD_DAYS(Current_Date, -7)
ORDER BY T0.[DocDate]
```



8년 전 문서 데이터 조회

```mssql
SELECT T0.[CardCode], T0.[CardName], T0.[DocTotal], T0.[DocNum] 
FROM OPOR T0 WHERE T0.[DocStatus]  = 'C' 
and T0.[DocDate] >= DateAdd(YY, -8, getDate())
ORDER BY T0.[DocDate]
```



##### 매개변수 사용 

특정 기간 동안의 문서 조회

```mssql
SELECT T0.[CardCode], T0.[CardName], T0.[DocTotal], T0.[DocNum] 
FROM OPOR T0 WHERE T0.[DocStatus]  = 'C' 
and T0.[DocDate] >= [%0] and T0.[DocDate] <= [%1]
ORDER BY T0.[DocDate]
```

```mssql
SELECT T0.[CardCode], T0.[CardName], T0.[DocTotal], T0.[DocNum] 
FROM OPOR T0 WHERE T0.[DocStatus]  = 'C' 
and T0.[DocDate] between [%0] and [%1]
ORDER BY T0.[DocDate]	
```



#### 질의 생성기의 질의요소 - Group By절

특정고객에 대한 input이 있고 연도별, 고객별 doctotal summary를 출력, 정렬 하기 위해 사용 

정확하지 않음. 정정 부탁드려요.

```mssql
SELECT COUNT(T0.[CardCode]), T0.[CardName], SUM(T0.[DocTotal]), T0.[DocNum] 
FROM OPOR T0 WHERE T0.[DocStatus]  = 'C' 
and T0.[DocDate] between [%0] and [%1]
GROUP BY DATEPART(T0.[DocDate],year)
ORDER BY T0.[DocDate]	
```

```mssql
SELECT YEAR(T0.[DocDate]) AS YEAR, T0.[CardName], SUM(T0.[DocTotal])
FROM OPOR T0
WHERE T0.[CardName] = '[%0]'
GROUP BY YEAR(T0.[DocDate]), T0.[CardName]
ORDER BY YEAR(T0.[DocDate])
```



#### 조인

```mssql
SELECT T0.[CardCode]), T0.[CardName], T0.[DocNum], T0.[DocTotal], T0.[Balance], T1.[ListNum]
FROM OPOR T0 INNER JOIN OCRD T1 ON T0. [CardCode] = T1.[CardCode]
WHERE T0.[DocStatus]  = 'C' 
```



#### 활성 윈도우의 필드를 참조하는 질의

판매 오더의 고객/공급업체 코드에 해당하는 계정잔액 값을 불러오는 질의

```mssql
SELECT DISTINCT T0.[Balance]
FROM OCRD T0
INNER JOIN ORDR T1 ON T0.[CardCode] = T1.[CardCode]
WHERE ($[ORDR.CardCode] = T0.[CardCode])
```



### 사용자 정의 필드 UDF

#### 사용자 정의 필드 추가 

[툴 - 커스터마이징 툴 - 사용자 정의 필드 - 관리]

헤더레벨에 추가된 UDF는 처음 별도 윈도우 표시가 된다. [뷰 - 사용자 정의 필드]



#### 사용자 정의 필드 이동가능

예) 판매 오더 문서에서 계정 잔액 조회

1. 사용자 정의 필드를 추가 후
2.  질의 완성
3. 필드에 포커스를 두고  Shift + Alt + F2 필드에 적용할 쿼리 선택. (저장된 사용자 정의값)
4. 판매 문서에 포커싱하고 Tool - Edit Form UI - UDF 클릭 2초 동안 누르고 있으면 이동할 수 있다.



#### 사용자 정의 필드 등록정보

직접 만든 것은 'U_' 로 시작

###### SSMS 에서 조회

```mssql
use SBODemoKR;
select CardName, U_Acctbal
from ORDR
```



#### 사용자 정의 값에 질의 사용

사용하고자 하는 필드에 커서 올린 후 Alt + Shift + F2 를 눌러 추가

사용자 정의 값 자동 변경 필요 시 자동 새로고침 기능 이용

1. 저장된 사용자 정의 값 조회 (Default)
   - 질의가 한 번 실행되면 필드 내에 결과가 유지 된다.
2. 정기적 새로 고침
   - 종속 필드가 변경되거나 문서에서 선택될 때마다 질의를 실행한다.



### 사용자 정의 테이블 UDT

1. tool - customizing - UDT 만듦 : 접두부 ''@''로 시작
2. '' - UDF - User Tables에서 열 추가 [툴 - 커스터마이징 툴 - 사용자 정의 필드 - 관리]
3. UDF에 테이블을 연결 후 User Definded Windows 에서 사용자 정의 테이블 조회 가능. 이때, 테이블 이름은 @로 , 열은 UDF이므로 U_로 시작

🚨 Type이 No object일 때만 User Defined Window에서 조회 가능.



###### SSMS 에서 조회

```mssql
use SBODemoKR;
SELECT *  FROM [dbo].[@DRIVERS]  T0
```



### 사용자 정의 오브젝트(문서) UDO

헤더와 라인으로 이루어진 문서

1. UDT 생성창에서 타입을 Document와 Document Row 각각 생성

2. 신규테이블을 오브젝트 등록 마법사 이용 오브젝트 등록 [툴 - 커스터마이징 툴 - 오브젝트 등록 마법사]

3. Unique ID를 Deliveryrq로 지정.

   질의에서 조회시 SELECT * FROM [@Deliveryrq]

4. Type을 Doc

5. 마스터 오브젝트와 전표 형태로 만들어진 UDT만 UDO로 생성 가능하다.



### UI 구성

- Menu
- Form
- Item