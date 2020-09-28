# 실습

> ''엑셀은 위대하다'

## 1. 기본 CRUD

### Create

```mssql
CREATE TABLE CO03H (
column 1 nvarchar(10),
...
column n nvarchar(10)
)
```

> **Tip**
>
> 엑셀 raw 데이터에서 칼럼만 복사 후, 행 열 바꿔서 붙여넣기 후 데이터 타입 적어서
>
> SSMS에 붙여넣기.

### Insert

```mssql
Insert into CO03H select ('column 1', 'column 2' ... 'column n')
```

> **Tip**
>
> 엑셀 함수 concatenate()를 이용하여 셀 데이터 자동 채워서 SSMS에 붙여넣기
>
> 한글 값은 N '한글'과 같이 N을 앞에 붙인다.
>
> e.g.  
>
> =CONCATENATE("insert into co03H  select '",A6,"','",B6,"','",C6,"','",D6,"','",E6,"','",F6,"','",G6,"','",H6,"','",I6,"','",J6,"','",K6,"','",L6,"','",M6,"',N'",N6,"','",O6,"',N'",P6,"','",Q6,"','",R6,"',N'",S6,"','",T6,"','",U6,"','",V6,"','",W6,"'",)

### Update

```mssql
update co03h
set U_elnm = T1.U_ACCTNM
from co03h as T0 inner join co03l as T1
on T0.code = T1.code

select * from co03h;
```



## 2. 실습

---

### 판매오더

```mssql
select T0.CardCode, T0.CardName, T0.DocDate, T0.TaxDate,    T0.DocDueDate, T1.itemCode, T1.itemName, T1.Quantity, T1.whsCode ...
    from ordr as T0
    inner join rdr1 as T1
    on T0.docEntry = T1.docEntry
```

> Alt + F1로 Table 볼 수 있음.
>
> Query Generator로 필드값 쉽게 볼 수 있음.

### 복합 쿼리

```mssql
SELECT T0.CardCode
	,t0.CardName
	, (case when CardType = 'S' then N'벤더'
	when CardType = 'C' then N'고객' else N'리드' end ) as '고객 유형'
	,t2.CardType
	,t0.DocDate
	,t0.TaxDate
	,t0.DocDueDate
	,t1.ItemCode
	,t1.Dscription
	,t1.Quantity
	,t1.UseBaseUn
	,t1.WhsCode
	,t1.Price
	,t1.LineTotal
	--,( t1.Quantity * t2.AvgPrice) as 총계
FROM ordr t0
INNER JOIN rdr1 t1 ON t0.DocEntry = t1.DocEntry
inner join ocrd t2 on t0.CardCode = t2.CardCode
inner join oitm t3 on t1.ItemCode = t3.ItemCode
inner join owhs t4 on t1.WhsCode = t4.WhsCode
```



### 프로시저

```mssql
create PROCEDURE [dbo].[test] (
	@card AS NVARCHAR(20)
	,@itemcode AS NVARCHAR(20)
	,@date AS DATETIME
	)
AS
BEGIN
	SELECT t0.CardCode, t0.CardName, t0.DocDate, t0.TaxDate, t0.DocDueDate
		,t1.ItemCode
	FROM ordr t0
	INNER JOIN rdr1 t1 ON t0.DocEntry = t1.DocEntry
	WHERE t0.cardcode = @card
		AND t1.ItemCode = @itemcode
		AND t0.docdate > @date
END
```



### 뷰

```mssql
CREATE VIEW mView2
AS
SELECT ROW_NUMBER() OVER (
		ORDER BY docdate
		) AS 번호
	,cardcode
	,cardname
	,docdate
	,taxdate
	,docduedate
	,docstatus
	,(doctotal - vatsum) AS '할인 전 총계'
FROM opor
WHERE docdate BETWEEN dateadd(month, - 1, getdate())
		AND getdate()

SELECT *
FROM mView2


```