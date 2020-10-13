### SBO_SP_TransactionNotification

▶ SAP에서 데이터를 수정하는 방법이 다양하기 때문에 데이터 검증을 위해 사용한다.



#### 입력 변수

- @object_type : 트랜잭션 알림을 호출하는 개체 유형
- @Transaction_type : 추가 또는 업데이트와 같은 트랜잭션 유형
  - A : Add
  - U : Update
  - C : Cancel
  - L : Close
- @num_of_cols_in_key : 키에 있는 열의 개수 (일반적으로 1개)
- @list_of_key_cols_tab_del : 키 이름
- @list_of_cols_val_tab_del : 키 값



#### 출력 변수

- @error : 오류 코드
- @error_message : 오류 메시지