## 함수

SELECT ABS(-100)
수식의 절대값을 돌려준다

SELECT ROUND(1234.5678,2) => 1234.57
자릿수를 올려서 돌려준다.

SELECT RAND()
0~1까지의 임의의 숫자를 돌려준다 


SELECT SUBSTRING(N'SQL Server 2020', 12,4)
지정한 위치부터 지정한 개수의 문자를 돌려준다. 

SELECT LEFT('SQL Server 2020', 3), RIGHT('SQL Server 2020',4)
왼쪽 오른쪽 지정 위치부터 지정한 수만큼을 돌려준다. 

SELECT LEN('SQL Server 2020')
문자열의 길이를 돌려준다.

SELECT LOWER('abcdEFGH'), UPPER('abcdEFGH')
소문자를 대문자로, 대문자를 소문자로 변경한다. 

SELECT LTRIM(' 공백 '), RTRIM(' 공백 ')
왼쪽/ 오른쪽 공백 문자를 제거해 준다. 

SELECT REPLACE('SQL Server 2020','Server', '서버')
문자열의 내용을 지정한 것으로 찾아서 바꾼다. 

SELECT REPLICATE('SQL',5)
문자열을 지정한 수만큼 반복한다. 

SELECT GETDATE()
현재 날짜와 시간을 돌려준다.

SELECT DATEADD(DAY, 100, '2010-10-10') -***
2010년 10월 10일부터 100일 후의 날짜를 돌려준다. 
DAY 대신 YEAR, MONTH, WEEK, HOUR, MINUTE, SECOND가 올 수 있다. 

SELECT DATEDIFF(DAY, GETDATE(),'2020-10-19')
현재부터 2020년 10월 19일 까지 남은일을 알려준다.  두 날짜의 차이를 돌려준다. 

SELECT DATENAME(weekday, '2020-10-19')
2022년 10월 19일이 무슨요일인지 알려준다. 날짜의 지정한 부분만 돌려준다. 

SELECT MONTH('2022-10-19')
지정된 날짜의 일 월 년을 돌려준다. DAY(), MONTH() , YEAR()



### 서브쿼리

##### 단일행 서브쿼리 : 

납품 문서중 전기일이 제일 나중인 것 
SELECT * FROM ODLN A WHERE A.DocDate= (SELECT MAX(DocDate) FROM ODLN)

품목마스터에서 재고가 가장 많은 품목
SELECT * FROM OITM WHERE OnHand= (SELECT MAX(OnHand) FROM OITM)



##### 다중행 서브쿼리 : 

20200901~10100910 기간동안의 납품내역이 있는 BP 마스터 조회
SELECT * FROM OCRD WHERE CardCode IN (SELECT CardCode FROM ODLN WHERE DocDate BETWEEN '20200901' AND '20200910')



##### 스칼라 서브쿼리 : 

SELECT CardCode
, (SELECT CardName FROM OCRD WHERE CardCode = A.CardCode ) AS CardName
, DocDate
, TaxDate
, DocDueDate
FROM ODLN A

(=)

SELECT A.CardCode
, B.CardName
, DocDate
, TaxDate
, DocDueDate
FROM ODLN A
INNER JOIN OCRD B ON A.CardCode= B.CardCode

납품되었던(과거) 데이터 와 현재 데이터가 다를수 있다. 그럴땐 JOIN해서 지금의 품목 마스터를 보여준다. 
=> 최종 데이터의 이름으로 보여준다. 
SELECT A.ItemCode, Dscription B.ItemName
 FROM DLN1 A
INNER JOIN OITM B ON A.ItemCode = B. ItemCode



##### 인라인뷰 

--20200101~20200930 납품(ODLN)을 기반으로한 반품(ORDN)이 된 납품문서(DNL1) ( 반품은 납품문서를 원본으로 만들어진 것만 가져온다.)
SELECT B.* --B.DocEntry , B.LineNum,C.DocEntry, C.LineNum
FROM ODLN AS A
INNER JOIN DLN1 B ON A.DocEntry = B.DocEntry
INNER JOIN (SELECT BaseEntry, Baseline, DocEntry, LineNum
			FROM RDN1 
			WHERE DocDate BETWEEN '20200901' AND '20200930'
			AND Basetype=15) AS C on A.DocEntry = C.BaseEntry
	AND B.Linenum= C.Linenum



### UNION

중복값 제거
SELECT '1','2'
UNION
SELECT '1','2'



중복값 제거안함 ; 더 많이 사용함
SELECT '1','2'
UNION ALL
SELECT '1','2'



SELECT CONVERT(NVARCHAR(6),'20200923',112)

SELECT TOP 5 '반품', A.CardCode, B.ItemCode, B.Quantity
FROM ODLN A
INNER JOIN DLN1 B ON A.DocEntry = B.DocEntry 
--WHERE CONVERT(NVARCHAR(6), A.DocDate, 112)= '202006'
UNION ALL
SELECT TOP 5 '반품', A.CardCode, B.ItemCode, -1*(B.Quantity)
FROM ORDN A
INNER JOIN DLN1 B ON A.DocEntry = B.DocEntry 
--WHERE CONVERT(NVARCHAR(6), A.DocDate,112) = '202006'



### PIVOT 

잘안씀.

월별, 품목별 합계를 보고 싶을 때 

SELECT * 
FROM(
SELECT CONVERT(NVARCHAR(6), A.DocDate, 112) YYYYMM
, B.ItemCode
, B.Quantity
FROM ODLN A 
INNER JOIN DLN1 B ON A.DocEntry = B.DocEntry 
) AS DATA
PIVOT( SUM(Quantity) FOR DATA.YYYYMM IN ( [202005], [202006], [202007], [202008] ,[202010])) AS PVT
ORDER BY PVT.ItemCode ASC



SELECT * 
FROM(
SELECT CONVERT(NVARCHAR(6), A.DocDate, 112) YYYYMM
, B.ItemCode
, B.Quantity
FROM ODLN A 
INNER JOIN DLN1 B ON A.DocEntry = B.DocEntry 
) AS DATA
PIVOT( SUM(Quantity) FOR DATA.ItemCode IN ( [I00007] )) AS PVT
ORDER BY PVT.ItemCode ASC



### 실습

##### --1. 품목마스터에서 가장 품명(description | itemname)이 긴 품목 찾기 

==>답
SELECT top 1 ItemCode , LEN(ItemName) FROM OITM
ORDER BY LEN(ItemName) DESC



##### --2. 20200910 기준으로 전월 말일은? LEFT OR SUBSTRING 사용

==>답1 
SELECT SUBSTRING('20200910',1,6)+'01'	--2020001
==>답2
SELECT LEFT('20200910',6)+'01'			--2020001



##### --3. 20200910 기준으로 해당월 말일은?

==>답
SELECT DATEADD(D,-1,DATEADD(M,1,LEFT('20200910',6)+'01')) --20200930



##### --4. 오늘 기준 전월 말일은? GETDATA()사용

==>답
SELECT DATEADD(D,-1,CONVERT(NVARCHAR(6),GETDATE(),112)+'01')



##### --5. 오늘 기준 이번달 말일은? GETDATA() 사용

==>답
SELECT DATEADD(D,-1,DATEADD(M,1,CONVERT(NVARCHAR(6),GETDATE(),112)+'01'))



##### --6. 서브쿼리를 사용해서 한 번도 납품을 하지 않는 판매 품목 조회(판매품목 조건 : SellItem = 'Y')

==>답
SELECT * FROM OITM
WHERE SellItem = 'Y'
AND ItemCode NOT IN (select ItemCode FROM DLN1 )



##### --7. 20200101~20200930 기간 동안의 반품이 된 입고 po 문서 ? (반품은 입고 po 문서를 원본으로 만들어 진 것만 가져온다.)

=>답: 
SELECT B.* --B.CardCode , B.CardName, C.DocEntry, C.LineNum
FROM OPDN AS A
INNER JOIN PDN1 B ON A.DocEntry = B.DocEntry
INNER JOIN (SELECT BaseEntry, Baseline, DocEntry, LineNum
			FROM RPD1 
			WHERE DocDate BETWEEN '20071010' AND '20200930'
			AND Basetype=20) AS C on A.DocEntry = C.BaseEntry
								AND B.Linenum= C.Linenum



##### --8. 아래 쿼리를 활용하여 아래 데이터도 같이 출력해주세요. 서브쿼리 사용(인라인뷰)

OCRD--BP마스터의 CardName
OITM --품목마스터 ItemName

SELECT TOP 5 N'반품', A.CardCode, B.ItemCode, B.Quantity
FROM ODLN A
INNER JOIN DLN1 B ON A.DocEntry = B.DocEntry 
--WHERE CONVERT(NVARCHAR(6), A.DocDate, 112)= '202006'
UNION ALL
SELECT TOP 5 N'반품', A.CardCode, B.ItemCode, -1*(B.Quantity)
FROM ORDN A
INNER JOIN DLN1 B ON A.DocEntry = B.DocEntry 
--WHERE CONVERT(NVARCHAR(6), A.DocDate,112) = '202006'

=>답: 
 SELECT A.CardCode, B.CardName, A.ItemCode, C.ItemName, A.Quantity FROM(
SELECT TOP 5 N'납품' AS Type, A.CardCode, B.ItemCode, B.Quantity
FROM ODLN A
INNER JOIN DLN1 B ON A.DocEntry = B.DocEntry 
WHERE CONVERT(NVARCHAR(6), A.DocDate, 112)= '20010101'

UNION ALL

SELECT TOP 5 N'반품', A.CardCode, B.ItemCode, -1*(B.Quantity)
FROM ORDN A
INNER JOIN DLN1 B ON A.DocEntry = B.DocEntry 
WHERE CONVERT(NVARCHAR(6), A.DocDate,112) = '20200930') A

INNER JOIN OCRD B ON A.CardCode = B.CardCode
INNER JOIN OITM C ON A.ItemCode = C.ItemCode