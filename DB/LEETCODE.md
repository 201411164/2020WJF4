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



### Q. 262

```mssql
IF NOT EXISTS(select 1 from sysobjects where name='Trips' and xtype='U')
create table Trips(Id INT, Client_Id INT, Driver_Id INT, City_Id INT, STATUS varchar(25), Request_at VARCHAR(50))
alter table trips with check add constraint ck_status check(status in ('completed', 'cancelled_by_driver', 'cancelled_by_client'))

IF NOT EXISTS(select 1 from sysobjects where name = 'Users' and xtype = 'U')
create table Users(Users_Id INT, Banned VARCHAR(50), ROLE varchar(10))
alter table users add constraint ck_users_role check(role in('client', 'driver', 'partner'))
TRUNCATE TABLE Trips


INSERT INTO Trips (Id, Client_Id, Driver_Id, City_Id, STATUS, Request_at)
VALUES ('1', '1', '10', '1', 'completed', '2013-10-01')

INSERT INTO Trips (Id, Client_Id, Driver_Id, City_Id, STATUS, Request_at)
VALUES ('2', '2', '11', '1', 'cancelled_by_driver', '2013-10-01')

INSERT INTO Trips (Id, Client_Id, Driver_Id, City_Id, STATUS, Request_at)
VALUES ('3', '3', '12', '6', 'completed', '2013-10-01')

INSERT INTO Trips (Id, Client_Id, Driver_Id, City_Id, STATUS, Request_at)
VALUES ('4', '4', '13', '6', 'cancelled_by_client', '2013-10-01')

INSERT INTO Trips (Id, Client_Id, Driver_Id, City_Id, STATUS, Request_at)
VALUES ('5', '1', '10', '1', 'completed', '2013-10-02')

INSERT INTO Trips (Id, Client_Id, Driver_Id, City_Id, STATUS, Request_at)
VALUES ('6', '2', '11', '6', 'completed', '2013-10-02')

INSERT INTO Trips (Id, Client_Id, Driver_Id, City_Id, STATUS, Request_at)
VALUES ('7', '3', '12', '6', 'completed', '2013-10-02')

INSERT INTO Trips (Id, Client_Id, Driver_Id, City_Id, STATUS, Request_at)
VALUES ('8', '2', '12', '12', 'completed', '2013-10-03')

INSERT INTO Trips (Id, Client_Id, Driver_Id, City_Id, STATUS, Request_at)
VALUES ('9', '3', '10', '12', 'completed', '2013-10-03')

INSERT INTO Trips (Id, Client_Id, Driver_Id, City_Id, STATUS, Request_at)
VALUES ('10', '4', '13', '12', 'cancelled_by_driver', '2013-10-03')

TRUNCATE TABLE Users

INSERT INTO Users (Users_Id, Banned, ROLE)
VALUES ('1', 'No', 'client')

INSERT INTO Users (Users_Id, Banned, ROLE)
VALUES ('2', 'Yes', 'client')

INSERT INTO Users (Users_Id, Banned, ROLE)
VALUES ('3', 'No', 'client')

INSERT INTO Users (Users_Id, Banned, ROLE)
VALUES ('4', 'No', 'client')

INSERT INTO Users (Users_Id, Banned, ROLE)
VALUES ('10', 'Yes', 'driver')

INSERT INTO Users (Users_Id, Banned, ROLE)
VALUES ('11', 'No', 'driver')

INSERT INTO Users (Users_Id, Banned, ROLE)
VALUES ('12', 'No', 'driver')

INSERT INTO Users (Users_Id, Banned, ROLE)
VALUES ('13', 'No', 'driver')

--CODE
select request_at as Day, cast(count(case when status <> 'completed' then 1.0 else null end) /cast(count(request_at) as float) as decimal(16,2)) as 'Cancellation Rate'
from trips t 
inner join users u 
    on u.banned = 'No' and t.client_id = u.users_id  
where request_at between '2013-10-01' 
and '2013-10-03' 
group by request_at

```

```mssql
SELECT    Request_at AS 'Day'
        , ROUND((CONVERT(FLOAT,(SUM(CASE
                                        WHEN Status = 'completed' THEN 0
                                         ELSE 1
                                   END)))/COUNT(*)),2) AS 'Cancellation Rate'
FROM    Trips
WHERE   Client_Id IN (SELECT Users_Id FROM Users WHERE Banned = 'NO' AND Role = 'client')
        AND Driver_Id IN (SELECT Users_Id FROM Users WHERE Banned = 'NO' AND Role = 'driver')    
        AND Request_at BETWEEN ('2013-10-01') AND ('2013-10-03')
GROUP   BY  Request_at
```

