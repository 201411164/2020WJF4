### 저장 프로시저 실습

#### WJS_SP_EMP

##### Desciption

@GBN 매개변수 값이 F이거나 입력하지 않을 경우 입사일이 2020-12-31 이전인 사원에 대한 근속개월수 계산

@GBN 매개변수에 그 외의 값을 입력 시 EMP 테이블의 U_RERIRDT 컬럼의 값을 U_JOINDT의 년에 10을 더하여 UPDATE

@EMPNM 매개변수 값 입력 시 그 문자열로 시작하는 U_EMPNM 컬럼의 값들의 행을 SELECT

```mssql
ALTER	PROCEDURE [dbo].[WJS_SP_EMP]
(
	@GBN	AS NVARCHAR(10) = 'F'
	, @EMPNM	AS NVARCHAR(100)
)
AS

BEGIN

	SET NOCOUNT ON;
	
	IF	@GBN = 'F'
	BEGIN
	
			SELECT	CONVERT(INT, ROW_NUMBER() OVER(ORDER BY U_EMPNO)) AS '#'
					, U_EMPNO
					, U_EMPNM
					, U_JOINDT
					, U_RETIRDT
					, CASE	WHEN	U_JOINDT <= '2020-12-31'	THEN
							[dbo].[WJS_FN_WORKMONTH] (U_JOINDT, U_RETIRDT)
							ELSE
								NULL
							END	AS	U_MONTH
			FROM	EMP
			WHERE	U_EMPNM	LIKE @EMPNM + '%'
			ORDER	BY U_EMPNO
	
	END
	ELSE
	BEGIN
	
			UPDATE	EMP
			SET		U_RETIRDT = DATEADD(YEAR, 10, U_JOINDT)
			FROM	EMP
	
			--UPDATE	T0
			--SET		T0.U_RETIRDT = DATEADD(YEAR, 10, T0.U_JOINDT)
			--FROM	EMP	AS T0
	END

END
```



### 함수 실습

#### WJS_FN_WORKMONTH

##### Desciption

@JOINDT 매개변수의 값과 @RETIRDT 매개변수의 값을 DATEDIFF 날짜 함수를 이용하여 MONTH의 차이를 반환한다.

@JOINDT는 입사일자, @RETIRDT는 퇴사일자를 뜻한다.

```mssql
ALTER	FUNCTION	[dbo].[WJS_FN_WORKMONTH] (
	  @JOINDT		DATETIME
	, @RETIRDT		DATETIME
)

RETURNS		INT
AS
BEGIN

	DECLARE	@MONTH	AS INT
	
	SET	@MONTH = DATEDIFF(MONTH, @JOINDT, @RETIRDT)
	
	RETURN (@MONTH)

END
```

