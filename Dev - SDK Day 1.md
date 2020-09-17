# SDK  9.3 impl 13 14 15

## 프로세스 정리

캡파 + 코스트 = 원가

SDK

Integration framework 대신 B1F.

Service Layer : 기존 설치형 클라이언트 서비스가 아니라 server단, DI서버에서 발전, 분산 처리에 강함.



### 마스터 데이터

- BP

- MM

- 품목 그룹

  - 상품

  - 제품

    반제품(중간재)
    - 원재료 (박스; 개)수량 관리
    - 원료 : 단위로 관리

- GL 계정과목.

- 가격 리스트

### 생산

감가상각 내용 연수?

생산에 소비된 재료의 분개

이동평균으로 금액 재료비 또는 WIP (Work In Process)



| 생산 | 차                                       | 대           |
| ---- | ---------------------------------------- | :----------- |
| 오더 | WIP / 재료비 (이동 평균 금액)            | 원재료       |
| 입고 | 제품                                     | 직 간접 비용 |
|      | 제조원가(노무비, 재료비, 경비 ) 표준원가 | WIP          |

원가 = 제조원가 + 영업비용(= 판매비 + 관리비)



# 질의

### 조건

8년 전 문서 데이터 조회

```mssql
SELECT T0.[CardCode], T0.[CardName], T0.[DocTotal], T0.[DocNum] 
FROM OPOR T0 WHERE T0.[DocStatus]  = 'C' 
and T0.[DocDate] >= DateAdd(YY, -8, getDate())
ORDER BY T0.[DocDate]
```

### 변수 사용

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

### Group by 사용

정확하지 않음. 정정 부탁드려요.

```mssql
SELECT COUNT(T0.[CardCode]), T0.[CardName], SUM(T0.[DocTotal]), T0.[DocNum] 
FROM OPOR T0 WHERE T0.[DocStatus]  = 'C' 
and T0.[DocDate] between [%0] and [%1]
GROUP BY DATEPART(T0.[DocDate],year)
ORDER BY T0.[DocDate]	
```



### 사용자 정의 필드 UDF

#### 판매 오더 문서에서 계정 잔액 조회

1. 사용자 정의 필드를 추가
2.  질의 완성
3. 필드에 포커스를 두고  Shift + Alt + F2 필드에 적용할 쿼리 선택.
4. 판매 문서에 포커싱하고 Tool - Edit Form UI - UDF 클릭 2초 동안 누르고 있으면 이동할 수 있다.

### 사용자 정의 테이블 UDT

1. tool - customizing - UDT 만듦
2.  '' - UDF - User Tables에서 열 추가
3. User Definded Windows 에서 사용자 정의 테이블 조회 가능

🚨 Type이 No object일 때만 User Defined Window에서 조회 가능.



### 사용자 정의 오브젝트(문서) UDO

헤더와 라인으로 이루어진 문서

1. UDT 생성창에서 타입을 Document와 Document Row 각각 생성
2.  오브젝트 등록 마법사 이용 오브젝트 등록
3. Unique ID를 Deliveryrq로 지정.
4. Type을 Doc

### UI 구성

- Menu
- Form
- Item