내용 추가 : https://answers.sap.com/questions/9892157/difference-between-posttransnotificaion-and-transn.html  (참조링크)

위 링크의 답변에 따르면 차이점은 다음과 같습니다





 SP_TransactionNotification 

SP_PostTransactionNotice 

의 사용 용도는 검증 조건으로 같아서 사용 용도로 구분하기보다, 실행에서 차이가 난다고 합니다.



SBO_SP_TransactionNotification = When @error is different from 0, rollback the transaction and show @error_message in Business One Client



SBO_SP_PostTransactionNotice = When @error is different from 0, register @error and @error_message in Business One log files. No alerts are showed in Business One Client.



즉, SP_TransactionNotification의 경우에는 파일 추가하기 전에 검증, POSTTransactionNotice는, 파일 추가 후에 검증입니다.







### SP_TransactionNotification

- 모든 트랜잭션에 대한 Notification 수신
- 검증을 수행하여 검증 조건이 실패한 경우 사용자가 작업을 수행하지 못하게 함

https://blogs.sap.com/2015/01/27/sbosptransactionnotification/



### SP_PostTransactionNotice

- 외부의 프로그램을 호출하는 데 사용된다. (TransactionNotification과의 차이점)

  ex) 송장 추가 시 다른 DB에 즉시 동기화 할 시