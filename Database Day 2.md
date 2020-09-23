# SQL

```mssql
use db실습;
create table wj_prc(
[empcd] nvarchar(20) primary key,
[empnm] nvarchar(20) ,
[jikgb] nvarchar(20),
[sal] numeric(19,6)
)

insert into wj_prc (empcd, empnm, jikgb, sal)
select '10001', N'박보검',N'사원',100000
insert into wj_prc (empcd, empnm, jikgb, sal)
select '10002', N'공유',N'대리',200000
insert into wj_prc (empcd, empnm, jikgb, sal)
select '10003', N'손예진',N'과장',300000
insert into wj_prc (empcd, empnm, jikgb, sal)
select '10004', N'이준기',N'차장',400000
insert into wj_prc (empcd, empnm, jikgb, sal)
select '10005', N'이지은',N'사원',500000

Alter table wj_prc add yyyymm nvarchar(6) null;
alter table wj_prc add exeyn char(1) not null constraint df_exeyn default 'N';
alter table wj_prc add seq INT identity(1,1);
alter table wj_prc alter column jikgb nvarchar(30);
exec sp_rename 'wj_prc.exeyn', 'execyn', 'column';
 -- truncate table wj_prc;
select * from wj_prc;
/* 다국어 선언, 입력시에는 Nvarchar와 N''로 선언한다. */
```

### Delete 와 Truncate의 차이점

둘 다 데이터 삭제이지만, Truncate는 설정값까지 삭제함.

여기서 말하는 설정값은 NULL, NOT NULL, DEFAULT 등을 말함.

```mssql
SELECT * FROM WJ_PRC WHERE empcd not in ('10001', '10002', '10003') 
-- not in은 속도가 느리니 데이터가 많을 경우는 left join 사용할 것.
```



### 🚨MSSQL 주의점

- select 시 table에 **with** (no lock)을 쓴다. (select read만 하므로, lock 걸 필요 없기 때문에)
- 되도록이면 칼럼 생성시 char, varchar를 사용하고, Nchar, Nvarchar는 자제한다.
- 길이 fix된 칼럼의 경우는 var보다는 char를 사용하자.
- select 시  *를 사용하면 심각한 퍼포먼스 저하 때문에 사용하지 않는다.
- 전체 count를 조건 없이 가져오려면, **sys 테이블**을 사용한다. (sys 테이블이 최적화된 테이블 인듯)

```mssql
insert into wj_prc values('10006', N'박서준', N'차장', 350000,'202009');	
```

- WHERE절의 **왼쪽 칼럼**은 변형하지 않는다.
- COUNT 함수 보다는 **EXIST**를 사용하라.
- LEFT OUTER 조인은 되도록 사용하지 않는다.
- SELECT 절에 서브쿼리 보다는 **INNER JOIN**을 사용한다.
- DISTINCT 보다는 **GROUP BY**를 사용한다.



### Alt + F1 

테이블 조회 가능함.

### 실습

```mssql
USE SBODemoKR;
--1.
SELECT TOP 1 T0.SlpCode
	,t1.SlpName
	,sum(T1.Commission) AS [평균]
FROM ORDR T0
INNER JOIN OSLP T1 ON t0.SlpCode = t1.SlpCode
GROUP BY T0.SlpCode
	,t1.SlpName
ORDER BY [평균]

--2.
SELECT sum(t1.Quantity) AS [합계]
	,t1.ItemCode
	,year(t0.DocDate) AS [연도]
FROM ordr t0
INNER JOIN rdr1 t1 ON t0.DocEntry = t1.DocEntry
GROUP BY year(t0.docdate)
	,t1.ItemCode
ORDER BY year(t0.docdate)
	,t1.ItemCode

--3.
SELECT convert(nvarchar(6), t0.docdate, 112) AS [년월]
	, t1.SlpCode
	, t1.SlpName
	, sum(t2.QtyToShip)
FROM ordr t0
INNER JOIN oslp t1 ON t0.slpcode = t1.SlpCode
INNER JOIN rdr1 t2 ON t0.DocEntry = t2.docentry
group by convert(nvarchar(6), t0.docdate, 112), t1.SlpName, t1.SlpCode
order by t1.SlpCode, t1.SlpName, convert(nvarchar(6), t0.docdate, 112)


```

