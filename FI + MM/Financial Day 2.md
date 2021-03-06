# TB1100 재무 프로세스



## 통화관리 ( 기준정리 )
- 통화관리는 영구세팅 
- 계정 통화는 언제든 모든 통화로 변경 가능 (모든 통화-> 달러로 바꾸는 것은 못하지만, / 달러-> 모든 통화로 바꾸는 것은 가능하다.)
- 한 번 갱신한 후에는 현지 통화나 특정 외화로 다시 변경 불가
- 데이터베이스를 사용하기 시작한 후에는 현지 통화나 시스템 통화를 변경할 수 없음



#### [LC] local Currency : 현지 통화

회사가 법적으로 장부에 기록할 때 사용하는 통화

장부에 기록되는 통화



회사가 법적으로 장부에 기록할 때 사용하는 통화
=======
- 장부에 기록되는 통화



#### 시스템 통화

- 현지 통화와 다를 수 있음
- 다국적 기업의 지사에 쓰이는 통화이다.
- 로컬 통화 외에 추가로 기록되는 통화
- 모든 전기는 현지 통화로 자동 계산, 시스템 통화로 된 추가 계정잔액이 실시간으로 관리된다.



#### 모든 통화

- 모든 통화로 계정을 설정 가능, 기본 입력값은 현지 통화지만 어느 외화로든 분개 처리 가능
- 로컬 외에 통화 2가지를 관리할 경우??-> 모든 통화



#### [FC] Foreign Currency ; 외화

- 로컬 통화가 아닌 특정 추가 통화로 관리할 수 있다.

- 기본적으로 로컬 통화를 시스템 통화로 지정하여 관리한다.

- 판매 송장의 경우 BP통화(외화)를 디폴트로 관리된다.



###### 계정 통화

|           | 분개 입력 통화    | 계정 잔액 통화          | 내부 조정 (하나의 통화로) |
| --------- | ----------------- | ----------------------- | ------------------------- |
| 현지 통화 | 현지통화          | 현지, 시스템            | 현지 통화                 |
| 특정 외화 | 현지, 지정 외화   | 현지, 시스템, 지정 외화 | 지정 외화                 |
| 모든 통화 | 현지, 임의의 외화 | 현지, 시스템            | 현지 통화                 |



## 외화 환산 손익

- 지급되는 시점에 외환 차익 매긴다.
- 외환 환산 손익 차익 : 결산 시점, 구매나 판매 세팅에서  외화 환율이 자동 전개가 된다.
- 미결된 송장들을 가져와서 송장 시 환율과 결산 시 환율을 비교하여 손익 계산한다.
- 이익일 경우 대변, 손해일 경우 차변
- 손익 발생 시, G/L, A/R, A/P로 나누어서 계정과목을 관리한다.
- 외화 환산 손익 : 환율 차이에 따른 손실 또는 이익
- 환율차이거래는 현지통화로, 외화거래는 현지통화로 동시에 
- BP의 GL 계정 경우 현지, 특정, 모든 통화 가능하다. 
- 동일 거래처, 통화 2개 사용하는 경우의 회사가 사용하는 통화 :  모든 통화




##### 환산 차이 : 로컬 통화와 시스템 통화가 다를 경우, 사용하는 설정이다.

- 환산차이 윈도우 : 시스템통화와 로컬통화를 비교 vs  환산손익 위도우 : 그때 들어간 FC 금액과 비교 

- 로컬 통화와 시스템 통화가 다를 경우 외화 환산 손익이 발생하는 것
                => 특정 일자(=특정 기간의 마감일)로 자동으로 재평가 가능
- 외화 환산 손익과 같은 방식으로 진행하며 전기할 분개를 제안
- 환산 차이는 '시스템 통화'로만 전기



전기/만기/납품/증빙일

라인??



## 분개 전기


### 수동분개

- 문서의 출처 IN(AR), JE(분개), PU(AP)

- JE는 수동분개한 문서를 말한다. 

- 일반 계정, 관리 계정 따로 관리해야함.

-  1. 전기 템플릿 생성 : 수동방식, 구조 비슷, 직접입력, 옵션을 백분율로 입력
   2. 반복 전기 생성 : 반수동방식, 팝업창을 주기적으로 나오게(자동적), 주기에대한 옵션,일자를 숫자로 입력


#### 역분개 트랜잭션 (자동으로 표시)

- 분개를 취소하는 것
- 표준 역분개 트랜잭션 / 마이너스 금액을 사용한 트랜잭션 (일반적으로 쓰인다.)
- 스케줄 예약이 가능하며, 해당 날짜에 시스템 로그온 시 팝업 창으로 알려줌
- 활용 용도 : 1.오류 정정, 2.마감 분개를 조정하기 위해서 ( 마감 분개 전표 생성)



##### 표준 역분개 트랜잭션

- 반대쪽 총액이 증가



##### 마이너스금액을 사용한 역분개 트랜잭션

- 동일한 분개에 마이너스만 표시



관리계정과 일반 계정 차이 공부할 것.

##### 관리계정은 조회가 안 됨. 거래처를 통해서 조회가 가능함 이 때 Ctrl + Tab 쓰임. / 일반계정은 tab



## 반복 전기 (자동으로 표시)

 반복적으로 발생되는 전기를 특정 주기마다 전기되게 셋팅함 ex)임대료

 정기적으로 생성되는 유사한 고정 금액 분개

 시스템에 로그온 할 때 전기 대상 거래를 알려주는 리포트 제공



- ### 주기별

  주기적으로 반복되는 고정 금액 분개 시에 

  로그온 시 알려줌.

- ### 백분율

  구조가 매우 비슷한 분개

  전기 템플릿 : 반복적으로 일어나는 전기를 템플릿으로 만든다.

  



## 분개장 (자주 쓰임)

-  초안작성 시에 쓰이는 임시 분개이며 승인 절차를 밟는 분개 시에도 사용된다.(승인템플릿에 o)

- 차변과 대변이 일치하지 않아도 등록이 가능하다.

- 분개는 등록순간 수정이 불가능하지만 분개장은 수정 및 갱신이 가능하다.

- 분개레포트에서 전기 o

- 취소옵션을 사용하지 않는 이유: 1. 분개 마케팅문서에서 자동으로 생성되거나 / 2. 이미 취소되었을 때 



#### 분개장 전기 : 분개장을 영구 분개로 옮김.



## 전기기간


- 회계연도 연 단위 (역년으로)

- 하위기간 월 단위

  

### 기결산
-  수익(때변)과 비용(차변)으로 클리어하여, 단기 순이익 계산 하는 것.
- 회계연도 연 단위 (역년으로)
- 하위기간 월 단위
- 기결산 :  수익(때변)과 비용(차변)으로 클리어하여, 단기 순이익 계산 하는 것.
  연결산;
- G/L계정은 특정 기간 동안 활성화되도록 설정할 수도 있다.



## 내부 조정

- 수동 분개로 송장 미결 금액을 처리하면 참조하지 않고 처리하기 떄문에 계정 잔액 상에는 문제가 없지만, 관계맵에는 표시되지 않고 송장은 미결처리 된다.

- 송장 미결처리 방지를 위해서 내부 조정 프로세스를 거친다.

- 미결 차변 항목과 미결 대변 항목을 매칭하여 반제한다.

- 현지 통화로 내부 조정되나  따로 지정된 외화가 있을 경우는 외화로 조정된다.

- 모든 내부 조정 시 시스템 통화와 현지 통화로 대차 일치 해야 한다.
  (내부 조정은 하나의 통화로 수행)

- 환차일도 이곳에서 계산하게 된다.

- 송장할때와 입,지급이 맞지 않을때 둘을 이어 줄 수 있다. 미결된 잔액을 clear 해주기 위한 목적을 갖고 있다. => 합을 0으로 

- 같은 계정에 속한 대 차변을 연결해준다.

- 내부조정에 묶인 것은 내부조정안에서만 취소된다.

  

Q. 시스템 내부조정이 발생하는 트랜잭션 : 
- A / P 송장을 기준으로 공급업체에 대한 지급하는 경우 (지급에서 송장건을 선택했기 때문에 서로간 내부조정됨)
- 송장을 기반으로 하는 대변 메모 ( A/P 대변메모, A/R 대변메모 <-> Invoice, 즉 매출이 차감이 된다. 서로간 내부조정이 된다. ) 




## 연결된 비즈니스 파트너 : 복수 비즈니스 파트너로 조정한다.
- IN, PU를 맞춰서 0이 될 필요가 없다. 

- A: 10000 <-> B: 5000 인경우 A : 까고 5000만줘. 이런 경우가 될 수 있다. 

  


#### 내부조정이 쓰이는 특이 케이스

​	혼합거래처상에서 A/R과 A/P로 서로 반제, 상계하는 것.



> [혼합거래처] : 판매도 하고 구매도 하는 거래처
>
> [반제, 상계] : 퉁치는 것



Ctrl + A : 추가 모드로 변경

Ctrl + F : 찾기 모드로 변경





## 재무 보고서



의사결정과 신규 투자에 영향을 미침. 



### 계정과목표 구조



### 재무 보고서



#### 대차대조표(재무상태표) - 자산, 부채, 자본

- ​	대차대조표를 통해 단기간에 현금화할 수 있는 유동자산을 판단할 수 있다.
- ​	특정 시점(일반적으로 말일)에서의 자산, 부채, 자본의 상태를 나타낸 것

#### 시산표 = 대차대조표와 손익 계산서

#### 손익계산서 - 수익, 비용

​	특정 기간 동안 수익과 비용을 계산



러닝허브 배운 거 PDF 

https://help.sap.com/doc/978d6c0279534be0a5a2f7e167aa9b2e/B1_PRO_2.0/en-US/bdd8a366a1c04cf5bc233f0469ef9726.html?collapse=5
