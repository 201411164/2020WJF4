## DB 실습 정리 (20.09.25)

------

### 1. 데이터INSERTUPDATE.xlsx

#### 	1) CO03H (헤더 테이블 생성)

​		**(1)** Code, DocEntry, Canceled, Object, UserSign, Transfered, CreateDate, CreateTime, UpdateDate, 
​		 	UpdateTime, DataSource, U_CACD, U_ELCD, U_ELNM, U_ELGRP, U_ELGRPNM, U_CT, U_ELGBN, 
​			 U_ELGBNM, U_FIXCRT, U_VARCRT, U_USEYN, U_DIMYN라는 컬럼을 가지고 있는 테이블 생성

​		**(2)** 컬럼 부분을 복사하여 행과 열 바꿔서 붙여넣기 한 후, 옆의 열에 해당 컬럼의 자료형을 작성한다.

​		**(3)** 그대로 복사하여 SSMS에 붙여넣기 하여 생성한다.

```mssql
CREATE TABLE CO03H (
	 Code		INT PRIMARY KEY
	,DocEntry	INT
	,Canceled	CHAR(1)
	,Object		NVARCHAR(40)
	,UserSign	INT
	,Transfered	CHAR(1)
	,CreateDate	DATETIME
	,CreateTime	SMALLINT
	,UpdateDate	DATETIME
	,UpdateTime	SMALLINT
	,DataSource	CHAR(1)
	,U_CACD		NVARCHAR(100)
	,U_ELCD		NVARCHAR(100)
	,U_ELNM		NVARCHAR(100)
	,U_ELGRP	NVARCHAR(100)
	,U_ELGRPNM	NVARCHAR(100)
	,U_CT		NVARCHAR(100)
	,U_ELGBN	NVARCHAR(100)
	,U_ELGBNNM	NVARCHAR(100)
	,U_FIXCRT	NVARCHAR(100)
	,U_VARCRT	NVARCHAR(100)
	,U_USEYN	NVARCHAR(100)
	,U_DIMYN	NVARCHAR(100)
)
```



#### 	2) CO03L (디테일 테이블 생성)	

​		**(1)** Code, LineId, Object, U_ACCTCD, U_FMTCD, U_ACCTNM라는 컬럼을 가지고 있는 테이블 생성

​		**(2)** 헤더 테이블과 동일한 방법으로 생성한다.

```mssql
CREATE TABLE CO03L (
	 Code		INT PRIMARY KEY
	,LineId		INT
	,Object		NVARCHAR(40)
	,U_ACCTCD	NVARCHAR(100)
	,U_FMTCD	NVARCHAR(100)
	,U_ACCTNM	NVARCHAR(100)
)
```



#### 	3) 헤더 테이블과 디테일 테이블 Data INSERT

​		**(1)** CONCATENATE 함수를 사용하여 Excel의 행의 자료를 취합한다.

```excel
* 헤더 테이블 자료 취합 *
=CONCATENATE("INSERT INTO CO03H SELECT ","'",A4,"', '",B4,"', '",C4,"', '",D4,"', '",E4,"', '",F4,"', '","2011-06-23","', '",H4,"', '","2011-06-28","', '",J4,"', '",K4,"', '",L4,"', '",M4,"', N'",N4,"', '",O4,"', N'",P4,"', '",Q4,"', '",R4,"', N'",S4,"', '",T4,"', '",U4,"', '",V4,"', '",W4,"'")
```

```excel
* 디테일 테이블 자료 취합 *
=CONCATENATE("INSERT INTO CO03L SELECT ","'",A2,"', '",B2,"', '",C2,"', '",D2,"', '",E2,"', N'",F2,"'")
```

​		**(2)** '(1)'에서 취합한 헤더 테이블 자료를 SSMS에 붙여넣어 헤더 테이블에 INSERT 한다.

```mssql
INSERT INTO CO03H SELECT '1', '1', 'N', 'WJS_SCO03', '1', 'N', '2011-06-23', '1743', '2011-06-28', '1051', 'I', 'C1', '5211100', N'원재료비', 'PG05', N'제조노무비', 'P', 'U5211100', N'원재료비', '100', '0', 'Y', 'N'
INSERT INTO CO03H SELECT '2', '2', 'N', 'WJS_SCO03', '2', 'N', '2011-06-23', '1549', '2011-06-28', '1051', 'I', 'C1', '5221200', N'부재료비', 'PG05', N'제조노무비', 'P', 'U5221200', N'부재료비', '100', '0', 'Y', 'N'
INSERT INTO CO03H SELECT '3', '3', 'N', 'WJS_SCO03', '2', 'N', '2011-06-23', '1554', '2011-06-28', '1417', 'I', 'C1', '5231100', N'급여', 'PG05', N'제조노무비', 'P', 'U5231100', N'급여', '100', '0', 'Y', 'N'
INSERT INTO CO03H SELECT '4', '4', 'N', 'WJS_SCO03', '2', 'N', '2011-06-23', '1554', '2011-06-28', '1418', 'I', 'C1', '5231300', N'상여금', 'PG05', N'제조노무비', 'P', 'U5231300', N'상여금', '100', '0', 'Y', 'N'
INSERT INTO CO03H SELECT '5', '5', 'N', 'WJS_SCO03', '2', 'N', '2011-06-23', '1555', '2011-06-28', '1051', 'I', 'C1', '5231400', N'제수당', 'PG05', N'제조노무비', 'P', 'U5231400', N'제수당', '100', '0', 'Y', 'N'
INSERT INTO CO03H SELECT '6', '6', 'N', 'WJS_SCO03', '2', 'N', '2011-06-23', '1556', '2011-06-28', '1418', 'I', 'C1', '5231500', N'잡급', 'PG05', N'제조노무비', 'P', 'U5231500', N'잡급', '100', '0', 'Y', 'N'
INSERT INTO CO03H SELECT '7', '7', 'N', 'WJS_SCO03', '2', 'N', '2011-06-23', '1556', '2011-06-28', '1418', 'I', 'C1', '5232100', N'퇴직급여', 'PG05', N'제조노무비', 'P', 'U5232100', N'퇴직급여', '100', '0', 'Y', 'N'
INSERT INTO CO03H SELECT '8', '8', 'N', 'WJS_SCO03', '2', 'N', '2011-06-23', '1556', '2011-06-28', '1418', 'I', 'C1', '5232200', N'퇴직급여충당금전입', 'PG05', N'제조노무비', 'P', 'U5232200', N'퇴직급여충당금전입', '100', '0', 'Y', 'N'
INSERT INTO CO03H SELECT '9', '9', 'N', 'WJS_SCO03', '2', 'N', '2011-06-23', '1556', '2011-06-28', '1418', 'I', 'C1', '5232300', N'단퇴급여충당금전입', 'PG05', N'제조노무비', 'P', 'U5232300', N'단퇴급여충당금전입', '100', '0', 'Y', 'N'
INSERT INTO CO03H SELECT '10', '10', 'N', 'WJS_SCO03', '2', 'N', '2011-06-23', '1556', '2011-06-28', '1418', 'I', 'C1', '5241010', N'법정복리비', 'PG07', N'제조복리후생비', 'P', 'U5241010', N'법정복리비', '100', '0', 'Y', 'N'
INSERT INTO CO03H SELECT '11', '11', 'N', 'WJS_SCO03', '2', 'N', '2011-06-23', '1556', '2011-06-28', '1418', 'I', 'C1', '5241020', N'후생비', 'PG07', N'제조복리후생비', 'P', 'U5241020', N'후생비', '100', '0', 'Y', 'N'
INSERT INTO CO03H SELECT '12', '12', 'N', 'WJS_SCO03', '2', 'N', '2011-06-23', '1556', '2011-06-28', '1418', 'I', 'C1', '5241030', N'중식비', 'PG07', N'제조복리후생비', 'P', 'U5241030', N'중식비', '100', '0', 'Y', 'N'
INSERT INTO CO03H SELECT '13', '13', 'N', 'WJS_SCO03', '2', 'N', '2011-06-23', '1556', '2011-06-28', '1418', 'I', 'C1', '5241040', N'연월차수당', 'PG07', N'제조복리후생비', 'P', 'U5241040', N'연월차수당', '100', '0', 'Y', 'N'
INSERT INTO CO03H SELECT '14', '14', 'N', 'WJS_SCO03', '2', 'N', '2011-06-23', '1556', '2011-06-28', '1418', 'I', 'C1', '5241050', N'학자보조금', 'PG07', N'제조복리후생비', 'P', 'U5241050', N'학자보조금', '100', '0', 'Y', 'N'
INSERT INTO CO03H SELECT '15', '15', 'N', 'WJS_SCO03', '2', 'N', '2011-06-23', '1556', '2011-06-28', '1418', 'I', 'C1', '5241060', N'임직원경조금', 'PG07', N'제조복리후생비', 'P', 'U5241060', N'임직원경조금', '100', '0', 'Y', 'N'
INSERT INTO CO03H SELECT '16', '16', 'N', 'WJS_SCO03', '2', 'N', '2011-06-23', '1556', '2011-06-28', '1418', 'I', 'C1', '5241070', N'기타복리후생비', 'PG07', N'제조복리후생비', 'P', 'U5241070', N'기타복리후생비', '100', '0', 'Y', 'N'
INSERT INTO CO03H SELECT '17', '17', 'N', 'WJS_SCO03', '2', 'N', '2011-06-23', '1556', '2011-06-28', '1418', 'I', 'C1', '5241080', N'조직관리비', 'PG07', N'제조복리후생비', 'P', 'U5241080', N'조직관리비', '100', '0', 'Y', 'N'
INSERT INTO CO03H SELECT '18', '18', 'N', 'WJS_SCO03', '2', 'N', '2011-06-23', '1556', '2011-06-28', '1418', 'I', 'C1', '5241090', N'특근식비', 'PG07', N'제조복리후생비', 'P', 'U5241090', N'특근식비', '100', '0', 'Y', 'N'
INSERT INTO CO03H SELECT '19', '19', 'N', 'WJS_SCO03', '2', 'N', '2011-06-23', '1556', '2011-06-28', '1418', 'I', 'C1', '5242030', N'통신비', 'PG04', N'제조기타제조경비', 'P', 'U5242030', N'통신비', '100', '0', 'Y', 'N'
INSERT INTO CO03H SELECT '20', '20', 'N', 'WJS_SCO03', '2', 'N', '2011-06-23', '1556', '2011-06-28', '1418', 'I', 'C1', '5242040', N'수도광열비(상하수도)', 'PG04', N'제조기타제조경비', 'P', 'U5242040', N'수도광열비(상하수도)', '100', '0', 'Y', 'N'
INSERT INTO CO03H SELECT '21', '21', 'N', 'WJS_SCO03', '2', 'N', '2011-06-23', '1556', '2011-06-28', '1418', 'I', 'C1', '5242041', N'수도광열비(전기-일반)', 'PG04', N'제조기타제조경비', 'P', 'U5242041', N'수도광열비(전기-일반)', '100', '0', 'Y', 'N'
INSERT INTO CO03H SELECT '22', '22', 'N', 'WJS_SCO03', '2', 'N', '2011-06-23', '1556', '2011-06-28', '1418', 'I', 'C1', '5242042', N'수도광열비(전기-충전)', 'PG04', N'제조기타제조경비', 'P', 'U5242042', N'수도광열비(전기-충전)', '100', '0', 'Y', 'N'
INSERT INTO CO03H SELECT '23', '23', 'N', 'WJS_SCO03', '2', 'N', '2011-06-23', '1556', '2011-06-28', '1418', 'I', 'C1', '5242043', N'수도광열비(가스/석유)', 'PG04', N'제조기타제조경비', 'P', 'U5242043', N'수도광열비(가스/석유)', '100', '0', 'Y', 'N'
INSERT INTO CO03H SELECT '24', '24', 'N', 'WJS_SCO03', '2', 'N', '2011-06-23', '1556', '2011-06-28', '1418', 'I', 'C1', '5242050', N'교육훈련비', 'PG04', N'제조기타제조경비', 'P', 'U5242050', N'교육훈련비', '100', '0', 'Y', 'N'
INSERT INTO CO03H SELECT '25', '25', 'N', 'WJS_SCO03', '2', 'N', '2011-06-23', '1556', '2011-06-28', '1418', 'I', 'C1', '5242060', N'세금과공과금(주민세)', 'PG04', N'제조기타제조경비', 'P', 'U5242060', N'세금과공과금(주민세)', '100', '0', 'Y', 'N'
INSERT INTO CO03H SELECT '26', '26', 'N', 'WJS_SCO03', '2', 'N', '2011-06-23', '1556', '2011-06-28', '1418', 'I', 'C1', '5242061', N'세금과공과금(재산세)', 'PG04', N'제조기타제조경비', 'P', 'U5242061', N'세금과공과금(재산세)', '100', '0', 'Y', 'N'
INSERT INTO CO03H SELECT '27', '27', 'N', 'WJS_SCO03', '2', 'N', '2011-06-23', '1556', '2011-06-28', '1418', 'I', 'C1', '5243010', N'유형자산감가상각비(건물)', 'PG03', N'제조감가상각비', 'P', 'U5243010', N'유형자산감가상각비(건물)', '100', '0', 'Y', 'N'
INSERT INTO CO03H SELECT '28', '28', 'N', 'WJS_SCO03', '2', 'N', '2011-06-23', '1556', '2011-06-28', '1418', 'I', 'C1', '5243020', N'유형자산감가상각비(구축물)', 'PG03', N'제조감가상각비', 'P', 'U5243020', N'유형자산감가상각비(구축물)', '100', '0', 'Y', 'N'
```

​		**(3)** '(1)'에서 취합한 디테일 테이블 자료를 SSMS에 붙여넣어 디테일 테이블에 INSERT 한다.

```mssql
INSERT INTO CO03L SELECT '1', '1', 'WJS_SCO03', '5211100', '5211100', N'유형자산감가상각비(기계장치)'
INSERT INTO CO03L SELECT '2', '1', 'WJS_SCO03', '5221200', '5221200', N'유형자산감가상각비(공구와기구)'
INSERT INTO CO03L SELECT '3', '1', 'WJS_SCO03', '5231100', '5231100', N'유형자산감가상각비(차량운반구)'
INSERT INTO CO03L SELECT '4', '1', 'WJS_SCO03', '5231300', '5231300', N'유형자산감가상각비(비품-일반)'
INSERT INTO CO03L SELECT '5', '1', 'WJS_SCO03', '5231400', '5231400', N'유형자산감가상각비(비품-주차타워)'
INSERT INTO CO03L SELECT '6', '1', 'WJS_SCO03', '5231500', '5231500', N'무형자산감가상각비'
INSERT INTO CO03L SELECT '7', '1', 'WJS_SCO03', '5232100', '5232100', N'사용권자산감가상각비(리스)'
INSERT INTO CO03L SELECT '8', '1', 'WJS_SCO03', '5232200', '5232200', N'토지임차료(작업동)'
INSERT INTO CO03L SELECT '9', '1', 'WJS_SCO03', '5232300', '5232300', N'토지임차료(실내보관동)'
INSERT INTO CO03L SELECT '10', '1', 'WJS_SCO03', '5241010', '5241010', N'토지임차료(야드)'
INSERT INTO CO03L SELECT '11', '1', 'WJS_SCO03', '5241020', '5241020', N'수선비'
INSERT INTO CO03L SELECT '12', '1', 'WJS_SCO03', '5241030', '5241030', N'보험료(화재/재산-작업동)'
INSERT INTO CO03L SELECT '13', '1', 'WJS_SCO03', '5241040', '5241040', N'보험료(화재/재산-보관동/주차타워)'
INSERT INTO CO03L SELECT '14', '1', 'WJS_SCO03', '5241050', '5241050', N'보험료(산재/상해)'
INSERT INTO CO03L SELECT '15', '1', 'WJS_SCO03', '5241060', '5241060', N'보험료(배상-재난/사고)'
INSERT INTO CO03L SELECT '16', '1', 'WJS_SCO03', '5241070', '5241070', N'운반비'
INSERT INTO CO03L SELECT '17', '1', 'WJS_SCO03', '5241080', '5241080', N'도서인쇄비'
INSERT INTO CO03L SELECT '18', '1', 'WJS_SCO03', '5241090', '5241090', N'소모품비'
INSERT INTO CO03L SELECT '19', '1', 'WJS_SCO03', '5242030', '5242030', N'지급임차료'
INSERT INTO CO03L SELECT '20', '1', 'WJS_SCO03', '5242040', '5242040', N'용역비(광택)'
INSERT INTO CO03L SELECT '21', '1', 'WJS_SCO03', '5242041', '5242041', N'용역비(세차)'
INSERT INTO CO03L SELECT '22', '1', 'WJS_SCO03', '5242042', '5242042', N'용역비(기타)'
INSERT INTO CO03L SELECT '23', '1', 'WJS_SCO03', '5242043', '5242043', N'기타수수료(GS PDI 공통)'
INSERT INTO CO03L SELECT '24', '1', 'WJS_SCO03', '5242050', '5242050', N'기타수수료(기타)'
INSERT INTO CO03L SELECT '25', '1', 'WJS_SCO03', '5242060', '5242060', N'보관료(작업동)'
INSERT INTO CO03L SELECT '26', '1', 'WJS_SCO03', '5242061', '5242061', N'보관료(실내보관동)'
INSERT INTO CO03L SELECT '27', '1', 'WJS_SCO03', '5243010', '5243010', N'후생비'
INSERT INTO CO03L SELECT '28', '1', 'WJS_SCO03', '5243020', '5243020', N'중식비'
```



#### 	4) 헤더 테이블의 Data를 디테일 테이블의 Data로 UPDATE

​		**(1)** 데이터를 디테일 테이블의 U_ACCTM 필드를 헤더 테이블의 U_ELNM 필드로 UPDATE

```mssql
-- 웅진은 JOIN을 걸어서 사용한다.
UPDATE		T0
SET			U_ELNM = T1.U_ACCTNM
FROM		CO03H T0
INNER JOIN	CO03L T1 ON T0.Code = T1.Code
```



### 2. 실습교육.xlsx

#### 	1) 판매오더 조회

​		**(1)** 판매오더의 헤더 테이블과 라인 테이블을 JOIN하여 데이터 조회

​		**(2)** ORDR(판매오더 헤더) 테이블과 RDR1(라인) 테이블의 Primary Key인 DocEntry를 기준으로 JOIN

```mssql
SELECT		  T0.CardCode	AS '고객'
			, T0.CardName	AS '이름'
			, T0.DocDate	AS '전기일'
			, T0.DocDueDate	AS '납품임'
			, T0.TaxDate	AS '증빙일'
			, T1.ItemCode	AS '품목코드'
			, T1.Dscription	AS '품목명'
			, T1.Quantity	AS '수량'
			, T1.WhsCode	AS '창고'
			, T1.UseBaseUn AS '재고 단위'
			, T1.Price		AS '가격'
			, T1.LineTotal	AS '금액'
FROM		ORDR AS T0
INNER JOIN	RDR1 AS T1 ON T0.DocEntry = T1.DocEntry
ORDER		BY T0.CardCode
```

#### 	2) 복합쿼리

​		**(1)** 여러 테이블을 JOIN하여 데이터 조회

​		**(2)** ORDR(판매오더 헤더) / RDR1(판매오더 라인) / 비즈니스파트너(OCRD) / 품목마스터(OITM) /
​			 창고(OWHS) 테이블 JOIN

```mssql
SELECT		  T0.CardCode	AS '고객'
			, T0.CardName	AS '이름'
			, CASE	T3.CardType 
				WHEN 'C' THEN N'고객' 
				WHEN 'L' THEN N'리드'
				END AS '비즈니스 파트너 유형'
			, T0.DocDate	AS '전기일'
			, T0.DocDueDate	AS '납품임'
			, T0.TaxDate	AS '증빙일'
			, T1.ItemCode	AS '품목코드'
			, T2.ItemName	AS '품목명'
			, T1.Quantity	AS '수량'
			, T1.WhsCode	AS '창고'
			, T4.WhsName	AS '창고명'
			, T1.UseBaseUn	AS '재고 단위'
			, T1.Price		AS '가격'
			, T1.LineTotal	AS '금액'
FROM		ORDR AS T0
INNER JOIN	RDR1 AS T1 ON T0.DocEntry = T1.DocEntry	-- ORDR, RDR1 JOIN
INNER JOIN	OITM AS T2 ON T1.ItemCode = T2.ItemCode	-- RDR1, OITM JOIN
INNER JOIN	OCRD AS T3 ON T0.CardCode = T3.CardCode	-- ORDR, OCRD JOIN
INNER JOIN	OWHS AS T4 ON T1.WhsCode = T4.WhsCode	-- RDR1, OWHS JOIN
ORDER		BY T0.CardCode
```

#### 	3) 프로시져

​		**(1)** 고객, 품목코드, 전기일 보다 큰 날짜를 입력 받아 조건에 맞는 데이터 조회

​		**(2)** ORDR(판매오더 헤더) 테이블과 RDR1(라인) 테이블의 Primary Key인 DocEntry를 기준으로 JOIN

​		**(3)** LIKE 사용 시 주의 할 점!

​			**[1]** LIKE 사용 시 일부분만 입력해도 정보를 가져올 수 있다.
​			**[2]** ERP의 경우 조건에 맞는 데이터만 조회해야 하는 경우가 있다. (불필요한 데이터가 포함되면 안 된다.)
​			**[3]** 따라서 LIKE 사용 시 위 '[2]'의 경우를 생각하면서 사용 해야한다.

```mssql
CREATE PROCEDURE [dbo].[TEST] (
		  @CardCode		AS NVARCHAR(100)
		, @ItemCode		AS NVARCHAR(100)
		, @BigDocDate	AS DATETIME
)
AS
BEGIN

	SET NOCOUNT ON;
	
	SELECT T0.DocEntry
		, T0.CardCode	AS '고객'
		, T0.CardName	AS '이름'
		, T0.DocDate	AS '전기일'
		, T0.DocDueDate	AS '납품임'
		, T0.TaxDate	AS '증빙일'
		, T1.ItemCode	AS '품목코드'
		, T1.Dscription	AS '품목명'
		, T1.Quantity	AS '수량'
		, T1.WhsCode	AS '창고'
		, T1.UseBaseUn	AS '재고 단위'
		, T1.Price		AS '가격'
		, T1.LineTotal	AS '금액'
	FROM	ORDR AS T0
	INNER JOIN	RDR1 AS T1 ON T0.DocEntry = T1.DocEntry
	-- 입력한 값이 포함되어 있는 Data 조회
	WHERE	(T0.CardCode LIKE '%' + @CardCode + '%') AND
			(T1.ItemCode LIKE '%' + @ItemCode + '%') AND (T0.DocDate > @BigDocDate)
	-- 입력한 값과 일치하는 Data만 조회
	WHERE	(T0.CardCode = @CardCode) AND (T1.ItemCode = @ItemCode) AND
			(T0.DocDate > @BigDocDate)
	-- 둘 중 하나 선택하여 사용
	ORDER	BY T0.CardCode
	
	END

GO
```

#### 	4) 뷰

​		**(1)** 오늘부터 한 달 이전까지 생성된 구매오더 내역을 불러온다.

​		**(2)** 구매오더 테이블은 OPOR

​		**(3)** 뷰 관련 피드백

​			**[1]** 뷰는 INSERT, DELETE, UPDATE가 가능하다.
​			**[2]** 원본 테이블이 변경되어도 뷰의 정보는 변경되지 않는다.
​			**[3]** 따라서 잘 사용하지 않는다. (원본 테이블과 데이터 차이)

​		**(4)** 할인전 총계 관련 피드백

​			**[1]** 할인전 총계의 값은 라인테이블의 할인율은 적용되어진 값이다.
​			**[2]** 할인전 총계 밑의 할인율을 적용하지 않은 값이다.
​			**[3]** OPOR.DiscPrcnt가 할인율의 값을 가지게 되는 필드이다.
​			**[4]** 보통 금액을 구할 시 OPOR.DocCur(통화) 필드의 값을 같이 보여준다.

```mssql
CREATE VIEW [dbo].[BEFORE_MONTH_OPOR] AS 
SELECT	  ROW_NUMBER() OVER(ORDER BY CardCode)	AS '번호'
		, CardCode		AS '공급업체'
		, CardName		AS '이름'
		, DocDate		AS '전기일'
		, DocDueDate	AS '납품일'
		, TaxDate		AS '증빙일'
		, DocStatus		AS '문서 상태'
		, DocTotal - VatSum + DiscSum   AS '할인전 총계' 
FROM	OPOR
WHERE	DocDate > DATEADD(MONTH,-1,GETDATE())

GO
```

