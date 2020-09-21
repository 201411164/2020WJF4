**MS-SQL** 

> SQL문은 대소문자를 구분하지 않지만 데이타는 대문자와 소문자를 구분한다
> 주석을 다는 방법은 /* 주석 */ 이거나 한 줄만 주석 처리를 할 경우는 문장 맨 앞에 --를 붙인다
> 각각의 데이타베이스의 SYSOBJECTS 테이블에 해당 데이타베이스의 모든 정보가 보관되어 있다
> SYSOBJECTS의 TYPE 칼럼으로 
>
> 'U'=사용자 테이블, 'P'=저장 프로시저, 'K'=프라이머리 키, 'F'=포린 키,
> 'V'=뷰, 'C'=체크 제약 등 오브젝트 이름과 정보를 알 수 있다

 

 

 **데이타 검색**

```mssql
USE 데이타베이스명   /* USE 문을 사용한 데이타베이스 선택 */

 SELECT * FROM 데이블명   /* 모든 칼럼 불러오기 */

 SELECT TOP n * FROM 테이블명  /* 상위 n개의 데이타만 가져오기 */
 SELECT 칼럼1, 칼럼2, 칼럼3 FROM 테이블명 /* 특정 칼럼 가져오기 */
 SELECT 칼럼1 별명1, 칼럼2 AS 별명2 FROM 테이블명 /* 칼럼에 별명 붙이기 */
 SELECT 칼럼3 '별 명3' FROM 테이블명     /* 칼럼 별명에 스페이스가 들어갈 경우는 작은따옴표 사용 */
 SELECT DISTINCT 칼럼 FROM 테이블명           /* 중복되지 않는 데이타만 가져오기 */
 
 /* 데이타는 오름차순으로 재배열된다
   DISTINCT를 사용하면 재배열이 될때까지 데이타가 리턴되지 않으므로 수행 속도에 영향을 미친다 */
 SELECT * FROM 테이블명 WHERE 조건절           /* 조건에 해당하는 데이타 가져오기 */
  /* 조건식에 사용하는 비교는 칼럼=값, 칼럼!=값, 칼럼>값, 칼럼>=값, 칼럼<값, 칼럼<=값이 있다
   문자열은 ''(작은따옴표)를 사용한다
   날짜 비교를 할때는 'yy-mm-dd' 형식의 문자열로 한다(날짜='1992-02-02', 날짜>'1992-02-02')
   */
 SELECT * FROM 테이블명 WHERE 칼럼 BETWEEN x AND y /* 칼럼이 x>=와 y<=사이의 데이타 가져오기 */
 SELECT * FROM 테이블명 WHERE 칼럼 IN (a, b...)     /* 칼럼이 a이거나 b인 데이타 가져오기 */


 SELECT * FROM 테이블명 WHERE 칼럼 LIKE '패턴'      /* 칼럼이 패턴과 같은 데이타 가져오기 */
  -- 패턴에 사용되는 기호는 %, _가 있다
   'k%'(k로 시작되는), '%k%'(중간에 k가 있는), '%k'(k로 끝나는)
   'p_'(p로 시작하는 2자리), 'p___'(p로 시작하는 4자리), '__p'(3자리 데이타중 p로 끝나는)
```

 🚨**Like 패턴 주의 사항**



\- MSSQL LIKE 쿼리에서 와일드 카드(예약어) 문자가 들어간 결과 검색 시

언더바(_)가 들어간 결과를 보기 위해 아래처럼 쿼리를 날리니

 

```mssql
select * from 테이블명 where 컬럼명 like '%_%'
```

 

모든 데이터가 결과로 튀어나왔다. -_-;;

언더바가 와일드 카드(쿼리 예약어)이기 때문인데 이럴 땐

 

```mssql
select * from 테이블명 where 컬럼명 like '%[_]%'

 SELECT * FROM 테이블명 WHERE 칼럼 IS NULL        /* 칼럼이 NULL인 데이타 가져오기 */
 SELECT * FROM 테이블명 WHERE 칼럼 NOT BETWEEN x AND y  /* 칼럼이 x와 y 사이가 아닌 데이타 가져오기 */
 SELECT * FROM 테이블명 WHERE 칼럼 NOT IN (a, b...)   /* 칼럼이 a나 b가 아닌 데이타 가져오기 */
 SELECT * FROM 테이블명 WHERE 칼럼 NOT LIKE '패턴'    /* 칼럼이 패턴과 같지 않은 데이타 가져오기 */
 SELECT * FROM 테이블명 WHERE 칼럼 IS NOT NULL      /* 칼럼이 NULL이 아닌 데이타 가져오기 */
 SELECT * FROM 테이블명 WHERE 칼럼>=x AND 칼럼<=y    
 SELECT * FROM 테이블명 WHERE 칼럼=a or 칼럼=b
 SELECT * FROM 데이블명 WHERE 칼럼1>=x AND (칼럼2=a OR 칼럼2=b)
  ** 복수 조건을 연결하는 연산자는 AND와 OR가 있다
   AND와 OR의 우선순위는 AND가 OR보다 높은데 우선 순위를 바꾸고 싶다면 ()을 사용한다
 SELECT * FROM 테이블명 ORDER BY 칼럼          /* 칼럼을 오름차순으로 재배열하기 */
 SELECT * FROM 테이블명 ORDER BY 칼럼 ASC        
 SELECT * FROM 테이블명 ORDER BY 칼럼 DESC        /* 칼럼을 내림차순으로 재배열하기 */
 SELECT * FROM 테이블명 ORDER BY 칼럼1 ASC, 칼럼2 DESC  /* 복수 칼럼 재배열하기 */
 SELECT * FROM 테이블명 ORDER BY 1 ASC, DESC 3      /* 칼럼 순서로 재배열하기 */
     /* 기본적으로 SELECT 문에서는 출력순서가 보증되지 않기 때문에
   데이타의 등록 상태나 서버의 부하 상태에 따라 출력되는 순서가 달라질 수 있다
   따라서 출력하는 경우 되도록이면 ORDER BY를 지정한다
   칼럼 번호는 전체 칼럼에서의 번호가 아니라 SELECT문에서 선택한 칼럼의 번호이고 1부터 시작한다 */
```

 

 **연산자
** ** 1순위는 수치 앞에 기술되는 + - 같은 단항 연산자
   2순위는 사칙 연산의 산술 연산자인 * / + -
   3순위는 = > 비교 연산자
   4순위는 AND OR 같은 논리 연산자
   ()을 붙이면 우선 순위를 바꿀수 있다

 \1. SELECT 문의 연산
   SELECT 칼럼1, 칼럼2, 칼럼3+칼럼4 AS '별명' FROM 테이블명
 \2. ORDER BY 구의 연산
   SELECT 칼럼1, 칼럼2, 칼럼3+칼럼4 AS '별명' FROM 테이블명 ORDER BY 칼럼3+칼럼4 DESC
   SELECT 칼럼1, 칼럼2, 칼럼3+칼럼4 AS '별명' FROM 테이블명 ORDER BY 3 DESC
 \3. WHERE 구의 연산
   SELECT 칼럼1, 칼럼2, 칼럼3+칼럼4 AS '별명' FROM 테이블명 WHERE 칼럼2>=(칼럼3+칼럼4)
 \4. NULL 연산
   SELECT 칼럼1, 칼럼2, ISNULL(칼럼3, 0) + ISNULL(칼럼4, 0) AS '별명' FROM 테이블명
  ** 수치형 데이타와 NULL값과의 연산 결과는 항상 NULL이다
   만약 NULL 값을 원치 않으면 ISNULL(칼럼, 기준값) 함수를 사용해서 기준값을 변환시킨다
 \5. 날짜 연산 
   SELECT GETDATE()                  /* 서버의 현재 날짜를 구한다 */
   SELECT 날짜칼럼, 날짜칼럼-7 FROM 테이블명
   SELECT 날짜칼럼, 날짜칼럼+30 FROM 테이블명
   SELECT 날짜칼럼, DATEDIFF(day, 날짜칼럼, GETDATE()) FROM 테이블명
  ** 날짜의 가산과 감산은 + -로 할 수 있다
   날짜와 날짜 사이의 계산은 DATEDIFF(돌려주는값, 시작날짜, 끝날짜) 함수를 사용한다
 \6. 문자 연산
   SELECT 칼럼1 + 칼럼2 FROM 테이블명
   SELECT 칼럼 + '문자열' FROM 테이블명
   SELECT 칼럼1 + '문자열' + 칼럼2 FROM 테이블명
  ** 기본 연결은 문자와 문자이고 문자와 숫자의 연결은 CONVERT 함수를 사용해야 한다

 

 **함수
** 1. 수치 함수
   ROUND(수치값, 반올림위치)      /* 반올림 및 자르기 */
   ABS(수치 데이타)           /* 절대값 */  
   SIGN(수치 데이타)          /* 부호 */
   SQRT(수치값)             /* 제곱근 */
   POWER(수치값, n)           /* n승 */
 \2. **문자열 함수 정리**

 

**1) Ascii() - 문자열의 제일 왼쪽 문자의 아스키 코드 값을 반환(Integer)**

**예) SELECT Ascii('abcd')**

  **>> 결과는 a의 아스키 코드값인 97 반환**

 

**2) Char() - 정수 아스키 코드를 문자로 반환(Char)**

**예) SELECT Char(97)**

   **>> 결과는 a 반환**

 

**3) Charindex() - 문자열에서 지정한 식의 위치를 반환**

**예) SELECT Charindex('b','abcde')  >> 결과 : 2
   SELECT Charindex('b','abcde',2) >> 결과 : 2
   SELECT Charindex('b','abcde',3) >> 결과 : 0** 

​    **-- 인수값이 3개일때 마지막은 abcde 에서의 문자열 검색 시작위치를 말하며**

​      **2인경우는 bcde 라는 문자열에 대해서 검색**

​      **3인 경우는 cde 라는 문자열에 대해서 검색 하게 된다.**

 

**4) Difference() - 두 문자식에 SUONDEX 값 간의 차이를 정수로 반환**

**예) SELECT Difference('a','b')**

 

**5) Left() - 문자열에서 왼쪽에서부터 지정한 수만큼의 문자를 반환**

**예) SELECT Left('abced',3)    결과 >> 3**

 

**6) Len() - 문자열의 길이 반환**

**예) SELECT Len('abced')     결과>>5**

 

**7) Lower() - 대문자를 소문자로 반환**

**예) SELECT Lower('ABCDE')  결과 >> abcde**

 

**8) Ltrim() - 문자열의 왼쪽 공백 제거**

**예) SELECT Ltrim('  AB CDE')  결과>> AB CDE**

 

**9)Nchar() - 지정한 정수 코드의 유니코드 문자 반환**

**예) SELECT Nchar(20)    결과 >> **

 

 

**10) Replace - 문자열에서 바꾸고 싶은 문자 다른 문자로 변환**

**예) SELECT Replace('abcde','a','1')  결과>>1bcde**

 

**11) Replicate() - 문자식을 지정한 횟수만큼 반복**

**예) SELECT Replicate('abc',3)       결과>> abcabcabc**

 

**12) Reverse() - 문자열을 역순으로 출력**

**예)  SELECT Reverse('abcde')       결과>> edcba**

 

**13) Right() - 문자열의 오른쪽에서 부터 지정한 수 만큼 반환(Left() 와 비슷 )**

**예) SELECT Right('abcde',3)         결과>> cde**

 

**14)Rtrim() - 문자열의 오른쪽 공백 제거**

**예) SELECT Rtrim(' ab cde ')        결과>> ' ab cde' <-- 공백구분을위해 ' 표시**

 

**15) Space() - 지정한 수만큼의 공백 문자 반환**

**예) SELECT Space(10)         결과 >> '     '  -- 그냥 공백이 나옴**

   **확인을 위해서 SELECT 'S'+Space(10)+'E'  결과 >> S    E** 

 

**16) Substring() - 문자,이진,텍스트 또는 이미지 식의 일부를 반환**

**예) SELECT Substring('abcde',2,3)   결과>> bcd**

 

**17)Unicode() - 식에 있는 첫번째 문자의 유니코드 정수 값을 반환**

**예)SELECT Unicode('abcde')       결과 >> 97**

 

**18)Upper() - 소문자를 대문자로 반환**

**예) SELECT Upper('abcde')       결과>> ABCDE**

 

**※ 기타 함수 Tip**

 

**19) Isnumeric - 해당 문자열이 숫자형이면 1 아니면 0을 반환**

**>> 숫자 : 1 , 숫자X :0**

**예) SELECT Isnumeric('30')    결과 >> 1**

   **SELECT Isnumeric('3z')    결과 >> 0**

 

**20) Isdate() - 해당 문자열이 Datetime이면 1 아니면 0
\>> 날짜 : 1  , 날짜 X :0
예) SELECT Isdate('20071231')  결과 >> 1**

   **SELECT Isdate(getdate())   결과 >> 1
   SELECT Isdate('2007123')   결과 >> 0**

   **SELECT Isdate('aa')       결과 >> 0**

 

 

**※ 날짜및 시간함수 정리**

 

**getdate() >> 오늘 날짜를 반환(datetime)**

 

**1> DateAdd()  - 지정한 날짜에 일정 간격을 + 새 일정을 반환**

**예) SELECT Dateadd(s,2000,getdate())**

 

**2> Datediff() - 지정한 두 날짜의 간의 겹치는 날짜 및 시간 범위 반환**

**예)SELECT DateDiff(d,getdate(),(getdate()+31))**

 

**3> Datename() -지정한 날짜에 특정 날짜부분을 나타내는 문자열을 반환**

**예) SELECT Datename(d,getdate())**

 

**4> Datepart() -지정한 날짜에 특정 날짜부분을 나타내는 정수를 반환**

**예) SELECT Datepart(d,getdate())**

 ** 돌려주는값(약어)
   Year-yy, Quarter-qq, Month-mm, DayofYear-dy, Day-dd, Week-wk,
   Hour-hh, Minute-mi, Second-ss, Milisecond-ms
   SELECT DATEADD(dd, 7, 날짜칼럼) 

**>> Datename , Datepart 은 결과 값은 같으나 반환 값의 타입이 틀림.**

 

**5> Day() -지정한 날짜에 일 부분을 나타내는 정수를 반환**

**예) SELECT Day(getdate())   -- 일 반환**

   **SELECT Month(getdate())  -- 월 반환**

​    **SELECT Year(getdate())      -- 년 반환**   4. 형변환 함수
   CONVERT(데이타 타입, 칼럼)         /* 칼럼을 원하는 데이타 타입으로 변환 */
   CONVERT(데이타 타입, 칼럼, 날짜형 스타일) /* 원하는 날짜 스타일로 변환 */
   CAST(칼럼 AS 데이타 타입)         /* 칼럼을 원하는 데이타 타입으로 변환 */
  ** 스타일
   1->mm/dd/yy, 2->yy.mm.dd, 3->dd/mm/yy, 4->dd.mm.yy, 5->dd-mm-yy,
   8->hh:mm:ss, 10->mm-dd-yy, 11->yy/mm/dd, 12->yymmdd
   SELECT CONVERT(varchar(10), 날짜칼럼, 2)

 

 **그룹화 함수
**  SELECT COUNT(*) FROM 테이블명        /* 전체 데이타의 갯수 가져오기 */
  SEELECT COUNT(칼럼) FROM 테이블명      /* NULL은 제외한 칼럼의 데이타 갯수 가져오기 */
  SELECT SUM(칼럼) FROM 테이블명       /* 칼럼의 합계 구하기 */
  SELECT MAX(칼럼) FROM 테이블명       /* 칼럼의 최대값 구하기 */
  SELECT MIN(칼럼) FROM 테이블명       /* 칼럼의 최소값 구하기 */
  SELECT AVG(칼럼) FROM 테이블명       /* 칼럼의 평균값 구하기 */
 GROUP BY문
  SELECT 칼럼 FROM 테이블명 GROUP BY 칼럼 
  SELECT 칼럼1, SUM(칼럼2) FROM 테이블명 GROUP BY 칼럼1
  SELECT 칼럼1, COUNT(*) FROM 테이블명 GROUP BY 칼럼1
  SELECT 칼럼1, 칼럼2, MAX(칼럼3) FROM 테이블명 GROUP BY 칼럼1, 칼럼2
  ** GROUP BY를 지정한 경우 SELECT 다음에는 반드시 GROUP BY에서 지정한 칼럼 또는
    그룹 함수만이 올 수 있다


 **조건
**  SELECT 칼럼1, SUM(칼럼2) FROM 테이블명 GROUP BY 칼럼1 HAVING SUM(칼럼2) < a
  SELECT 칼럼1, SUM(칼럼2) FROM 테이블명 ORDER BY 칼럼1 COMPUTE SUM(칼럼2)
  ** HAVING:    그룹 함수를 사용할 경우의 조건을 지정한다
    HAVING의 위치: GROUP BY의 뒤 ORDER BY의 앞에 지정한다
    COMPUTE:    각 그룹의 소계를 요약해서 보여준다
           ORDER BY가 항상 선행해서 나와야 한다
 조건절의 서브 쿼리
  ** SELECT 또는 INSERTY, UPDATE, DELETE 같은 문의 조건절에서 SELECT문을 또 사용하는 것이다
   SELECT문 안에 또 다른 SELECT문이 포함되어 있다고 중첩 SELECT문(NESTED SELECT)이라고 한다
  ** 데이타베이스에는 여러명이 엑세스하고 있기 때문에 쿼리를 여러개 나누어서 사용하면 데이타의 값이
   달라질수 있기때문에 트랜잭션 처리를 하지 않는다면 복수의 쿼리를 하나의 쿼리로 만들어 사용해야 한다
  SELECT 칼럼1, 칼럼2 FROM 테이블명 WHERE 칼럼2 = (SELECT 칼럼2 FROM 테이블명 WHERE 조건)
  SELECT 칼럼1, 칼럼2 FROM 테이블명 WHERE 칼럼1 IN (SELECT 칼럼1 FROM 테이블명 WHERE 조건)
  ** 서브 쿼리에서는 다른 테이블을 포함할 수 있다
   두개의 테이블에서 읽어오는 서브쿼리의 경우 서브 쿼리쪽에 데이타가 적은 테이블을 주 쿼리쪽에 데이타가
   많은 테이블을 지정해야 처리 속도가 빨라진다
  SELECT 칼럼1, 칼럼2 FROM 테이블명 WHERE 칼럼1 IN (SELECT 칼럼2-1 FROM 테이블명2 WHERE 조건)
  ** FROM구에서 서브 쿼리를 사용할 수 있다
   사용시 반드시 별칭을 붙여야 하고 처리 속도가 빨라진다
  SELECT 칼럼1, 칼럼2 FROM 테이블명 WHERE 조건1 AND 조건2
  SEELCT 칼럼1, 칼럼2 FROM (SELECT 칼럼1, 칼럼2 FROM 테이블명 WHERE 조건1) 별칭 WHERE 조건2

 데이타 편집


 **추가
**  ** NULL 값을 허용하지도 않고 디폴트 값도 지정되어 있지 않은 칼럼에 값을 지정하지 않은채
   INSERT를 수행하면 에러가 발생한다
  ** 수치값은 그대로 문자값은 ''(작은따옴표)로 마무리 한다
  ** SELECT INTO는 칼럼과 데이타는 복사하지만 칼럼에 설정된 프라이머리, 포린 키등등의 제약 조건은
   복사되지 않기 때문에 복사가 끝난후 새로 설정해 주어야 한다

  INSERT INTO 테이블명 VALUES (값1, 값2, ...)   /* 모든 필드에 데이타를 넣을 때 */
  INSERT INTO 테이블명 (칼럼1, 칼럼2, ...) VALUES (값1, 값2, ...)  /* 특정 칼럼에만 데이타를 넣을 때 */
  INSERT INTO 테이블명 SELECT * FROM 테이블명2           /* 이미 존재하는 테이블에 데이타 추가 */
  INSERT INTO 테이블명(칼럼1, 칼럼2, ...) SELECT 칼럼1, 칼럼2, ...) FROM 테이블명2
  SELECT * INTO 테이블명 FROM 테이블명2               /* 새로 만든 테이블에 데이타 추가 */
  SELECT 칼럼1, 칼럼2, ... 테이블명 FROM 테이블명2

 **갱신
**  UPDATE 테이블명 SET 칼럼1=값1, 칼럼2=값2      /* 전체 데이타 갱신 */   
  UPDATE 테이블명 SET 칼럼1=값1, 칼럼2=값2 WHERE 조건        /* 조건에 해당되는 데이타 갱신 */

 

\- UPDATE~SELECT

UPDATE A
SET A.cyberLectures = B.bizAddress
FROM OF_Member A, OF_Member B
WHERE A.no = B.no



 **삭제
**  DELETE FROM 테이블명                       /* 전체 데이타 삭제 */
  DELETE FROM 테이블명 WHERE 조건    /* 조건에 해당되는 데이타 삭제 */

 

 **오브젝트
** ** 데이타베이스는 아래 오브젝트들을 각각의 유저별로 관리를 하는데 Schema(스키마)는 각 유저별 소유 리스트이다

 **1. Table(테이블)
**  ** CREATE일때 프라이머리 키를 설정하지 않는다면 (칼럼 int IDENTITY(1, 1) NOT NULL) 자동 칼럼을 만든다
   데이타들의 입력 순서와 중복된 데이타를 구별하기 위해서 반드시 필요하다
  ** 테이블 정보 SP_HELP 테이블명, 제약 조건은 SP_HELPCONSTRAINT 테이블명 을 사용한다

  CREATE TABLE 데이타베이스이름.소유자이름.테이블이름 (칼럼 데이타형 제약, ...) /* 테이블 만들기 */
  DROP TABLE 테이블명                              /* 테이블 삭제 */
  ALTER TABLE 테이블명 ADD 칼럼 데이타형 제약, ...               /* 칼럼 추가 */
  ALTER TABLE 테이블명 DROP COLUMN 칼럼                     /* 칼럼 삭제 */
  ** DROP COLUMN으로 다음 칼럼은 삭제를 할 수 없다
    \- 복제된 칼럼
    \- 인덱스로 사용하는 칼럼
    \- PRIMARY KEY, FOREGIN KEY, UNIQUE, CHECK등의 제약 조건이 지정된 칼럼
    \- DEFAULT 키워드로 정의된 기본값과 연결되거나 기본 개체에 바인딩된 칼럼
    \- 규칙에 바인딩된 칼럼
  CREATE TABLE 테이블명 (칼럼 데이타형 DEFAULT 디폴트값, ...)          /* 디폴트 지정 */
  CREATE TABLE 테이블명 (칼럼 데이타형 CONSTRAINT 이름 UNIQUE, ...)       /* 유니크 설정 */
  ** UNIQUE란 지정한 칼럼에 같은 값이 들어가는것을 금지하는 제약으로 기본 키와 비슷하지만
    NULL 값을 하용하는것이 다르다
  CREATE TABLE 테이블명 (칼럼 데이타형 CONSTRAINT 이름 NOT NULL, ...)      /* NOT NULL 설정 */
  CREATE TABLE 테이블명 (칼럼 데이타형 CONSTRAINT 이름 PRIMARY KEY, ...)    /* 기본 키 설정 */
  ** 기본 키는 유니크와 NOT NULL이 조합된 제약으로 색인이 자동적으로 지정되고 데이타를
    유일하게 만들어 준다
  ** 기본 키는 한 테이블에 한개의 칼럼만 가능하다
  CREATE TABLE 테이블명 (칼럼 데이타형 CONSTRAINT 이름 FOREIGN KEY REFERENCES 부모테이블이름(부모칼럼), ...)    
  CREATE TABLE 테이블명 (칼럼 데이타형 CONSTRAINT 이름 CHECK(조건), ...)    /* CHECK 설정 */
  ** CHECK는 조건을 임의로 정의할 수 있는 제약으로 설정되면 조건을 충족시키는 데이타만
    등록할 수 있고 SELECT의 WHERE구와 같은 조건을 지정한다
  ** CONSTRAINT와 제약 이름을 쓰지 않으면 데이타베이스가 알아서 이름을 붙이지만
    복잡한 이름이 되기 때문에 되도록이면 사용자가 지정하도록 한다
  ** CONSTRAINT는 칼럼과 데이타형을 모두 정의한 뒤에 맨 마지막에 설정할 수 있다
   CREATE TABLE 테이블명 (칼럼1 데이타형,
       칼럼2 데이타형, ...
       CONSTRAINT 이름 PRIMARY KEY(칼럼1)
              CONSTRAINT 이름 CHECK(칼럼2 < a) ...)    
  ALTER TABLE 테이블명 ADD CONSTRAINT 이름 제약문                /* 제약 추가 */
  ALTER TABLE 테이블명 DROP CONSTRAINT 제약명                  /* 제약 삭제 */
  ALTER TABLE 테이블명 NOCHECK CONSTRAINT 제약명                 /* 제약 효력 정지 */
  ALTER TABLE 테이블명 CHECK CONSTRAINT 제약명                  /* 제약 효력 유효 */
  ** 제약명은 테이블을 만들때 사용자가 지정한 파일 이름을 말한다

 

 **2. View(뷰)
**  ** 자주 사용하는 SELECT문이 있을때 사용한다
   테이블에 존재하는 칼럼들중 특정 칼럼을 보이고 싶지 않을때 사용한다
   테이블간의 결합등으로 인해 복잡해진 SELECT문을 간단히 다루고 싶을때 사용한다
  ** 뷰를 만들때 COMPUTE, COMPUTE BY, SELECT INTO, ORDER BY는 사용할 수 없고
   \#, ##으로 시작되는 임시 테이블도 뷰의 대상으로 사용할 수 없다
  ** 뷰의 내용을 보고 싶으면 SP_HELPTEXT 뷰명 을 사용한다

  CREATE VIEW 뷰명 AS SELECT문              /* 뷰 만들기 */
  CREATE VIEW 뷰명 (별칭1, 별칭2, ...) AS SELECT문  /* 칼럼의 별칭 붙이기 */
  CREATE VIEW 뷰명 AS (SELECT 칼럼1 AS 별칭1, 칼럼2 AS 별칭2, ...)
  ALTER VIEW 뷰명 AS SELECT문                    /* 뷰 수정 */
  DROP VIEW 뷰명                          /* 뷰 삭제 */
  CREATE VIEW 뷰명 WITH ENCRYPTION AS SELECT문           /* 뷰 암호 */
  ** 한번 암호화된 뷰는 소스 코드를 볼 수 없으므로 뷰를 암호화하기전에
    뷰의 내용을 스크립트 파일로 저장하여 보관한다
  INSERT INTO 뷰명 (칼럼1, 칼럼2, ...) VALUES (값1, 값2, ...)
  UPDATE 뷰명 SET 칼럼=값 WHERE 조건
  ** 원래 테이블에 있는 반드시 값을 입력해야 하는 칼럼이 포함되어 있지 않거나
    원래 칼럼을 사용하지 않고 변형된 칼럼을 사용하는 뷰는 데이타를 추가하거나
    갱신할 수 없다
  ** WHERE 조건을 지정한 뷰는 뷰를 만들었을때 WITH CHECK OPTION을 지정하지 않았다면
    조건에 맞지 않는 데이타를 추가할 수 있지만 뷰에서는 보이지 않는다
    또한 뷰를 통해서 가져온 조건을 만족하는 값도 뷰의 조건에 만족하지 않는 값으로도
    갱신할 수 있다
  CREATE VIEW 뷰명 AS SELECT문 WITH CHECK OPTION
  ** 뷰의 조건에 맞지 않는 INSERT나 UPDATE를 막을려면 WITH CHECK OPTION을 설정한다

 

 **3. Stored Procedure(저장 프로시저)
**  ** 데이타베이스내에서 SQL 명령을 컴파일할때 캐시를 이용할 수 있으므로 처리가 매우 빠르다
   반복적으로 SQL 명령을 실행할 경우 매회 명령마다 네트워크를 경유할 필요가 없다
   어플리케이션마다 새로 만들 필요없이 이미 만들어진 프로시저를 반복 사용한다
   데이타베이스 로직을 수정시 프로시저는 서버측에 있으므로 어플리케이션을 다시 컴파일할 필요가 없다
  ** 저장 프로시저의 소스 코드를 보고 싶으면 SP_HELPTEXT 프로시저명 을 사용한다

  CREATE PROC 프로시저명 AS SQL문  /* 저장 프로시저 */
  CREATE PROC 프로시저명 변수선언 AS SQL문 /* 인수를 가지는 저장 프로시저 */
  CREATE PROC 프로시저명 WITH ENCRYPTION AS SQL문 /* 저장 프로시저 보안 설정 */
  CREATE PROC 프로시저명             /* RETURN 값을 가지는 저장 프로시저 */
    인수1 데이타형, ... 인수2 데이타형 OUTPUT
  AS
    SQL문
    RETURN 리턴값
  DROP PROCEDURE 프로시저명1, 프로시저명2, ...  /* 저장 프로시저 삭제 */

  명령어
  BEGIN ... END  /* 문장의 블록 지정 */
  DECLARE @변수명 데이타형 /* 변수 선언 */
  SET @변수명=값  /* 변수에 값 지정 */
  PRINT @변수명  /* 한개의 변수 출력 */
  SELECT @변수1, @변수2  /* 여러개의 변수 출력 */
  IF 조건   /* 조건 수행 */
    수행1
  ELSE
    수행2        
  WHILE 조건1  /* 반복 수행 */
    BEGIN
      IF 조건2
     BREAK  - WHILE 루프를 빠져 나간다
     CONTINUE - 수행을 처리하지 않고 조건1로 되돌아간다
   수행
    END        
  EXEC 저장프로시저  /* SQL문을 실행 */
  EXEC @(변수로 지정된 SQL문)
  GO   /* BATCH를 구분 지정 */

 

  **에제
**  1. 기본 저장 프로시저
   CREATE PROC pUpdateSalary AS UPDATE Employee SET salary=salary*2


  \2. 인수를 가지는 저장 프로시저
   CREATE PROC pUpdateSalary
     @mul float=2, @mul2 int
   AS
     UPDATE Employee SET salary=salary* @Mul* @mul2
   EXEC pUpdateSalary 0.5, 2    /* 모든 변수에 값을 대입 */
   EXEC pUpdateSalary @mul2=2   /* 원하는 변수에만 값을 대입 */


  \3. 리턴값을 가지는 저장 프로시저
   CREATE PROC pToday
     @Today varchar(4) OUTPUT
   AS
     SELECT @Today=CONVERT(varchar(2), DATEPART(dd, GETDATE()))
     RETURN @Today
   DECLARE @answer varchar(4)
   EXEC pToday @answer OUTPUT
   SELECT @answer AS 오늘날짜


  \4. 변수 선언과 대입, 출력
   ** @는 사용자 변수이고 @@는 시스템에서 사용하는 변수이다

   DECLARE @EmpNum int, @t_name VARCHAR(20)
   SET @EmpNum=10

   SET @t_name = '강우정'
   SELECT @EmpNum

 

이런식으로 다중입력도 가능함.

SELECT @no = no, @name = name, @level = level
FROM OF_Member
WHERE userId ='"

 

 

 \4. Trigger(트리거)
  ** 한 테이블의 데이타가 편집(INSERT/UPDATE/DELETE)된 경우에 자동으로 다른 테이블의
   데이타를 삽입, 수정, 삭제한다
  ** 트리거 내용을 보고 싶으면 SP_HELPTRIGGER 트리거명 을 사용한다

  CREATE TRIGGER 트리거명 on 테이블명 FOR INSERT AS SQL문     /* INSERT 작업이 수행될때 */
  CREATE TRIGGER 트리거명 on 테이블명 AFTER UPDATE AS SQL문    /* UPDATE 작업이 수행되고 난 후 */
  CREATE TRIGGER 트리거명 on 테이블명 INSTEAD OF DELETE AS SQL문 
  DROP TRIGGER 트리거명

 

 \5. Cursor(커서)
  ** SELECT로 가져온 결과들을 하나씩 읽어들여 처리하고 싶을때 사용한다
  ** 커서의 사용방법은 OPEN, FETCH, CLOSE, DEALLOCATE등 4단계를 거친다
  ** FETCH에는 NEXT, PRIOR, FIRST, LAST, ABSOLUTE {n / @nvar}, RELATIVE {n / @nvar}가 있다

  SET NOCOUNT on   /* SQL문의 영향을 받은 행수를 나타내는 메시지를 숨긴다 */
  DECLARE cStatus SCROLL CURSOR  /* 앞뒤로 움직이는 커서 선언 */
  FOR
    SELECT ID, Year, City FROM aPlane
  FOR READ onLY
  OPEN cStatus   /* 커서를 연다 */
  DECLARE @ID varchar(50), @Year int, @City varchar(50), @Status char(1)
  FETCH FROM cStatus INTO @ID, @Year, @City /* 커서에서 데이타를 하나씩 가져온다 */
  WHILE @@FETCH_STATUS=0          /* 커서가 가르키는 결과의 끝까지 */
  BEGIN
    IF   @Year <= 5       SET @Status='A'
    ELSE IF @Year> 6 AND @Year <= 9 SET @Status='B'
    ELSE              SET @Status='C'
    INSERT INTO aPlane(ID, City, Status) VALUES(@ID, @Year, @Status)
    FETCH FROM cStatus INTO @ID, @Year, @City /* 커서에서 데이타를 하나씩 가져온다 */
  END
  CLOSE cStaus               /* 커서를 닫는다 */
  DEALLOCATE cStatus            /* 커서를 해제한다 */

 

 **보안과 사용자 권한
** ** 보안의 설정 방법은 크게 WINDOWS 보안과 SQL 보안으로 나뉘어 진다
 ** 사용자에게 역할을 부여하는데는 서버롤과 데이타베이스롤이 있다

 \1. SA(System Administrator)
  ** 가장 상위의 권한으로 SQL 서버에 관한 전체 권한을 가지고 모든 오브젝트를 만들거나
   수정, 삭제할 수 있다

 \2. DBO(Database Owner)
  ** 해당 데이타베이스에 관한 모든 권한을 가지며 SA로 로그인해서 데이타베이스에서 테이블을
   만들어도 사용자는 DBO로 매핑된다
  ** 테이블이름의 구조는 서버이름.데이타베이스이름.DBO.테이블이름이다

 \3. DBOO(Database Object Owner)
  ** 테이블, 인덱스, 뷰, 트리거, 함수, 스토어드 프로시저등의 오브젝트를 만드는 권한을 가지며
   SA나 DBO가 권한을 부여한다

 \4. USER(일반 사용자)
  ** DBO나 DBOO가 해당 오브젝트에 대한 사용 권한을 부여한다

 

 



 

 

-- ANY (OR 기능)

WHERE 나이 >= (SELECT 나이 FROM .......)

 

-- GROUP BY ALL (WHERE 절과 일치 하지 않는 내용은 NULL 로 표시)

SELECT 주소, AVG(나이) AS 나이 FROM MEMBER

WHERE 성별='남'

GROUP BY ALL 주소

-- 모든 주소는 나오며 성별에 따라 나이 데이터는 NULL

 

-- WITH ROLLUP

SELECT 생일, 주소, SUM(나이) AS 나이

FROM MEMBER

GROUP BY 생일, 주소 WITH ROLLUP

-- 생일 과 주소를 요약행이 만들어짐

 

-- WITH CUBE (위의 예제를 기준으로, 주소에 대한 별도 그룹 요약데이터가 하단에 붙어나옴)

 

-- GROUPING(컬럼명) ROLLUP 또는 CUBE 의 요약행인지 여부 판단(요약행이면 1 아니면 0)

SELECT 생일, 주소, GROUPING(생일) AS 생일요약행여부

 

-- COMPUTE (GROUP BY 와 상관없이 별도의 테이블로 요약정보 생성)

SELECT 생일, 나이

FROM MEMBER

COMPUTE SUM(나이), AVG(나이)

 

-- PIVOT (세로 컬럼을 가로 변경)

EX)

학년/ 반 / 학생수

 1   1   40

 1   2   45

 2   1   30

 2   2   40

 3   1   10

 3   2   10

위와 같이 SCHOOL 테이블이 있다면

 

SELECT 그룹할컬럼명, [열로변환시킬 행]

FROM 테이블

  PIVOT(

   SUM(검색할열)

   FOR 옆으로만들 컬럼명

​     IN([열로변환시킬 행])

​     ) AS 별칭

 

--실제 쿼리는

SELECT 학년, [1반], [2반]

FROM SCHOOL

  PIVOT(

   SUM(학생수)

   FOR 반

​     IN([1반], [2반])

​     ) AS PVT

 

-- UNPIVOT (가로 컬럼을 세로로)

 

SELECT 학년, 반, 학생수

FROM SCHOOL

 UNPIVOT(

  FOR 반

   IN( [1반], [2반] )

  ) AS UNPVT

 

-- RANK (순위)

SELECT 컬럼명, RANK() OVER( ORDER BY 순위 기준 컬럼명) AS 순위

FROM 테이블

 

-- PARTITION BY (그룹별로 순위 생성)

SELECT 컬럼명, RANK() OVER( PARTITION BY 그룹기준컬러명 ORDER BY 순위기준컬럼명) AS 순위

FROM 테이블

 

-- FULL OUTER JOIN (LEFT 조인과 RIGHT 조인을 합한것)

양쪽 어느 하나라도 데이가 있으면 나옴

 

 

-- ROW_NUMBER (순차번호 생성)

SELECT ROW_NUMBER() OVER( ORDER BY 기준열) AS 번호, 컬럼명

FROM 테이블











자료형 (데이터타입)

MSSQL 서버에서 사용하는 데이터 타입(자료형)은 두가지가 있다.

 

**1. 시스템에서 제공하는 System data type**







![](.\image/sql_자료형.jpg)





**성능향상을 위해서라면 가능한 작은 자료형을 사용하도록 하자.**

불필요하게 int를 쓰는 경우가 흔한데, 사용될 데이터의 범위를 생각해 본 후, 가장 작은 범위의 자료형을 사용하도록 하자.

 

 

**2. 사용자가 정의 하는 User data type**

 

사용자 정의 자료형이 왜 필요한가?

C언어를 비로한 몇 가지 언어에서 나타나는 사용자 정의 데이터 유형과 같다.

프로젝트에 참가하는 사람들이 동일한 데이터 타입을 사용하고자 원하거나,

한 컬럼에 대한 데이터 유형을 더 쉽게 사용하려고 할 때 적용시킬 수 있다.

 

사용 방법

sp_addtype [새로운 타입 이름], '[SQL 데이터 타입]'

 

예

sp_addtype empID, 'CHAR(10)'

sp_addtype empNO, 'CHAR(12)'

 

 

 

\* 참고로 자료형을 바꾸는 함수로는 **CONVERT()** 가 있다.

 

사용방법

SELECT CONVERT(CHAR(30), title) FROM BOOKS

--> title 라는 컬럼을 CHAR(30) 으로 변환하여 가져오는 것이다.

 

SELECT CONVERT(VARCHAR(10), 35)

--> 35 라는 숫자를 VARCHAR(10) 으로 변환한다. 

 

 

**흐름 제어문의 종류**

 

흐름 제어문이란 언어의 처리 순서를 변경하거나 변수를 선언하는 등의 문장을 말한다.

 

**○ GOTO 라벨**

 \- GOTO 를 만나면 라벨 부분으로 무조건 건너뛴다. 라벨은 라벨: 으로 정의한다.

예)

DECLARE...

SET...

table_label1:

.

.

IF .... GOTO table_label1

.

--> GOTO table_label1 을 만나면 table_label1: 부분으로 건너 뛴다.

 

**○ RETURN**

 \- RETURN 은 무조건 수행을 중지 하고 원래 호출된 곳으로 돌아간다.

 

**○ IF / ELSE** - 이름만 들어도 알만한 문법이다. 주의 할 점은 조건문 안의 SQL문장이 둘 이상이라면 BEGIN / END 로 묶어 준다.예)IF @begin > @end  BEGIN    SELECT * FROM 테이블1 WHERE 조건    RETURN  ENDELSE  SELECT * FROM......... **○ WHILE / BREAK / CONTINUE** - WHILE 다음에 조건으로 반복을 하게 되고,  BREAK 를 만나면 무조건 WHILE 을 벗어나고,  CONTINUE 를 만나면 무조건 WHILE 로 돌아간다.예)WHILE 조건BEGIN     반복하는 동안 실행할 문장들...     IF 조건        BREAK     IF 조건        CONTINUEEND **○ EXEC[UTE]** - EXEC 와 EXECUTE 는 같은 의미이다. - 두가지 용도로 사용되는데, - 첫 번째, 스토어드 프로시저를 실행할 때 사용한다.예)EXEC stored_procedure  - 두 번재, SQL 문장을 동적으로 변화시키며 수행할 수 있다.예)DECLARE @sql VARCHAR(255)SET @sql = 'SELECT COUNT(*) FROM 'SET @sql = @sql + 'titles 'EXEC(@sql)--> 실제 수행되는 문장은 SELECT COUNT(*) FROM titles 가 된다. **○ CASE** - 단순 CASE예)SELECT  CASE type    WHEN 'a' THEN 'Apple'    WHEN 'b' THEN 'Banana'    ELSE 'No Data'  END AS 과일, priceFROM titles  - 검색된 CASE예)SELECT title_id, qty AS '수량', CASE    WHEN qty >= 50 THEN 'A'    WHEN qty >= 30 THEN 'B'    ELSE 'C' END AS '등급'FROM titles **NULLIF** : 표현식 1과, 2를 비교>> 표현식 1과, 2를 비교 두 표현식이 같으면 NULL 을 리턴, 같지 않으면 표현식 1을 리턴
SELECT NULLIF(2,3) -- 2 리턴
SELECT NULLIF(3,3) -- NULL 리턴
 사용예 : 양쪽필드에서 수량이 같으면 NULL을 리턴하고 하니면 첫 필드의 값을 리턴할때  **COALESCE** : 뒤에 오는 표현식중에 처음으로 오는 NULL 이 아닌 값을 리턴 SELECT COALESCE(NULL, 3, 4) -- 3 리턴
SELECT COALESCE(1,NULL,) -- 1 리턴
SELECT COALESCE(NULL,NULL,4) -- 4 리턴SELECT COALESCE(NULL,NULL, NULL)--문법오류 사용예 : 하나만 값을 가지고 있는 컬럼에서 비교해서 값을 가져올때 매우 좋다 **SET** : 세성 옵션 (한번설정하면 세션이 끊어 질때까지 유용)=====================================================================================SET nocount OFF
 : 몇개 행이 처리 되었는지 결과보여주는 것을 설정한다 'SET rowcount [n]ex) SET rowcount 4
SELECT title_id FROM titles ORDER BY TITLESET rowcount 0 : 보여줄 목록의 행수를 선택한다. 목록의 정렬의 임의로 설정되므로 필요한 순서가 있다면 ORDER BY 를 사용해야 한다.
사용후엔 반드시   SET ROWCOUNT 0 을 이용해서 원위치 시켜놓아야 한다 ' ============================== **유니크 키 넣기** ==============================
ALTER TABLE 테이블명 ADD UNIQUE(컬럼1, 컬럼2)
ALTER TABLE 테이블명 DROP CONSTRAINT 유니크명