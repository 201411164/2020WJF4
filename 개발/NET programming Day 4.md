### 칼럼에 서브 쿼리 사용

```mssql
SELECT Docentry, (
		SELECT count(LineNum)
		FROM rdr1
		WHERE a.DocEntry = rdr1.docentry
		) AS num
FROM ordr a
```



### 특정 아이템 그룹 조회

```mssql
select top 109 B.ItmsGrpCod, A.*
from rdr1 A
inner join OITM as B
on a.ItemCode = b.ItemCode
where b.ItmsGrpCod = 104
```



### 변경 이력 테이블 조회

```mssql
SELECT CASE 
		WHEN A.LOGINSTANC = 0
			THEN 99999
		ELSE A.LOGINSTANC
		END AS LOGINSTANC, A.DOCENTRY, A.ITEMCODE, A.QUANTITY, CASE 
		WHEN a.Quantity = lag(A.Quantity, 1, (
					SELECT quantity
					FROM ado1
					WHERE docentry = 556
						AND LogInstanc = (
							SELECT max(loginstanc)
							FROM ado1
							)
					)) OVER (
				ORDER BY A.loginstanc
				)
			THEN N'같음'
		WHEN LogInstanc = 1
			THEN N'첫번째 문서'
		ELSE N'다름'
		END AS '이전 행과 비교'
FROM (
	SELECT LOGINSTANC, DOCENTRY, ITEMCODE, QUANTITY
	FROM RDR1
	WHERE ObjType = 17
		AND DocEntry = 556
	
	UNION ALL
	
	SELECT LOGINSTANC, DOCENTRY, ITEMCODE, QUANTITY
	FROM ADO1
	WHERE ObjType = 17
		AND DocEntry = 556
	) AS A
ORDER BY CASE 
		WHEN A.LogInstanc = 0
			THEN 99999
		ELSE A.LogInstanc
		END
```

