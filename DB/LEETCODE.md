### Q. fetch 2nd rows

```mssql
--A1 퍼포먼스가 좋다. max가 order보다 빠르기 때문이다.
select max(salary) as secondhiestsalary from employee
where salary < (select max(salary) from emplyee)

--A2 표준적인 방법이고, top과 offset은 동시에 사용할 수 없다.
select salary as SecondHighestSalary from employee
group by salary
order by salary desc
offset 1 rows
fetch next 1 rows only;

--A3 T-SQL 방법 select문으로 만든 가상의 from에다가 이름을 지어줘야 오류가 안난다.
--이 답안은 NULL 처리가 안 된다. ex) 테이블에 데이터가 1 row 있는 경우
select top 1 salary 
from (
    select top 2 salary
    from employee
    order by salary desc) topTwo order by salary

--A4 가장 RUNTIME 빠른 답안
SELECT	ISNULL(NULL, MAX(salary)) AS SecondHighestSalary
FROM	Employee
WHERE	Salary NOT IN (SELECT MAX(Salary)) FROM Employee)
```

### Q. Nth rows and null

```mssql
CREATE FUNCTION getNthHighestSalary(@N INT) RETURNS INT AS
BEGIN
    RETURN (
        /* Write your T-SQL query statement below. */
        select distinct salary
        from employee
        order by salary desc
        offset @n-1 rows
        fetch next 1 rows only       
    );
END
```

