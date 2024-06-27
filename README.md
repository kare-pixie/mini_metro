지하철 시뮬레이션 게임 [Mini Metro]를 Unity 2D로 모작한 게임입니다.

## 유튜브 링크
[![image](https://github.com/kare-pixie/mini_metro/assets/29856632/68b6c435-e078-4e75-ad7f-68a30dd3d6b8)
](https://youtu.be/RYgO2esJfAM)

## 프로젝트 주요 환경
유니티 버전 2020.3.36

## 기능 정의
1.맵 생성 및 관리<br>
 &nbsp;a.실제 지도 데이터를 바탕으로 맵 구현<br>
 &nbsp;b.역 위치와 연결 통로 설정<br>
 &nbsp;c.시간에 따른 맵 변화(역 증가)<br>
 
2.열차 운행 시스템<br>
&nbsp;a.노선별 열차 생성 및 이동 관리<br>
&nbsp;b.열차 속도, 정차시간 등 설정값에 따라 자동 운행<br>
&nbsp;c.노선 변경 등 입력 처리<br>
 
3.승객 시스템<br>
&nbsp;a.각 역에서 랜덤 승객 생성<br>
&nbsp;b.승객별 목적지 설정 및 경로 탐색 알고리즘 적용<br>
&nbsp;c.열차 탑승, 하차, 대기 중 상태 관리<br>
&nbsp;d.하차 시 도착 인원에 따라 점수 계산<br>
 
4.사용자 인터페이스<br>
&nbsp;a.마우스 클릭으로 노선 그리기 기능<br>
&nbsp;b.일시 정지, 시간 흐름 설정 기능<br>
 
5.자동 진행 및 패배 조건, 점수 기록<br>
&nbsp;a.시간 경과에 따른 자동 진행<br>
&nbsp;b.역에서 최대 승객 수 이상일 때 대기 시간을 초과하면 게임 오버<br>
&nbsp;c.점수 기록<br>
