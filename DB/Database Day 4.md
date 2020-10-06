### 1장

#### View

그뤱웨어 같은 타 시스템과 연동을 위해서 쓰임.

뷰의 특징은 유저에 따라서 특정 정보를 숨길 수 있다.

물리적인 저장소가 없는 가상의 데이터이며 호출하여 사용한다.



VS  ==> procedure 프로그램간 / view는 데이터를 호출해서 잠깐잠깐 쓸 때 , 한 환경에서 쓰이기 보다 타 시스템과 연동 할때 사용한다. 



#### Procedure

리턴값 없이 Result만을 보여주고, Function의 집합

처음 만들때는 CREATE , 생성 후 실행 시키고자 할 때는 ALTER

[간단한 프로시저 만들기]

```mssql
--프로시저 만들기 
USE SBODemoKR
GO


--CREATE -> ALTER
ALTER PROCEDURE [dbo].[WJS_SP_EMP]

		AS

		BEGIN
			SELECT	U_EMPNO,U_EMPNM,U_JOINDT,U_RETIRDT
			
			FROM EMP
		END
```



#### Function

작은 개념이고 리턴 값이 있다.

작은 기능을 의미함.

stored procedure 내에 계산해서 리턴값만 받고 싶을때 사용.=> 프로시저안에 펑션이 여러개 들어가 있을수 있다. 



#### Trigger

조건이 충족되면 자동으로 호출되는 것.



#### NOTI

SAP에서 쓰는 트리거

일종의 프로시저로 

조건에 충족하면 자동으로 호출되는 것. (분개장 / 분개 에 자동으로 내부에 호출 된다.)

🚨 의도하지 않은 노티를 방지하기 위해서 프로파일러로 디버깅을 한다.



#### CURSOR 커서

내부적으로 LOOP를 돌아 데이터를 INSERT 한다.



#### 동적 쿼리

조회하는 칼럼의 수가 동적으로 결정된다.

SELECT 하는 칼럼의 종류가 조건에 따라 달라질 때 사용한다.



#### OPEN SQL / QUERY

속도는 느리지만, 여러 DB 플랫폼에서 사용할 수 있는 API  => 외부 회사에 바로 인서트 하기 힘들 때 사용

이 기종 간 데이터베이스를 사용하고 싶을 때 사용한다.

ex) MS-SQL에서 HANA로 쿼리를 날릴 때 사용



#### 임시 테이블

```mssql
CREATE TABLE #TEMP()

DECLARE @TEMP AS TABLE()
```

TEMP라는 이름의 임시 테이블을 만들 때는 위와 같은 쿼리를 사용한다.

이 방식은 사용자들끼리 규칙으로 정해놓은 것이기 때문에 시스템적으로는 임시 테이블이 아니다. 따라서 해당 쿼리 사용 시 DROP TABLE로 임시 테이블을 삭제 해야한다.



### 2장

#### TRANSACTION

데이터가 대량으로 쓰이는 곳(원가 같은 곳)에서는 민감하다 

쿼리 실행 전이나 후에 트랜잭션을 걸어 결과를 미리 확인해보고 적용할지 말지를 선택한다.



-ROLLBACK : 해당 쿼리를 적용하지 않는다.

-COMMIT : 해당 쿼리를 적용한다.

-Transaction BEGIN TRAN 



### 동의어 Synonym

```mssql
USE [SBODemoKR]
GO

CREATE SYNONYM [dbo].[WJS_SP_EMP] FOR [WJS_SP_EMP].[dbo].[COMPANY]
GO
```

구문이 길어지는 것을 막기 위해 자주 사용하는 다른 DB의 테이블을 사용하기 위해서 사용하는 등록해서 사용하는 축약어.

동의어에 등록하여 사용.