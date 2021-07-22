# Assets/Scripts

모아정원에서 사용하는 모든 Script를 모아둔 폴더

> ⭐ 마크 : 주로 사용하므로 확인해 둘 필요가 있는 폴더/에셋
>
> ✔️ 마크 : 앱 개발에는 필요하나 자주 쓰이지 않으므로 필요할 때만 확인하면 되는 폴더/에셋
>
> ⛔ 마크 : 앱 개발에는 필요하지만, 굳이 확인할 필요는 없는 폴더/에셋

기본적으로 .meta 파일은 확인하지 않아도 된다.

---

- **⛔  AnimationSFX**

  Animation이 재생되면서 Sound가 재생되도록 하는 스크립트를 모아둔 폴더 (잘 쓰이지 않음)

  ---

- **⭐ CanvasHandler**

  캔버스를 컨트롤 할 수 있는 Handler를 모아둔 폴더

  ---

- **⛔ PopUpHandler**

  모아밴드 관련 팝업을 컨트롤하는 Handler를 모아둔 폴더 (이제 쓰이지 않음)

  ---

- **⭐ BluetoothManager.cs**

  Bluetooth관련 (연결 및 해제) 기능을 컨트롤하는 Handler

  ---

- **⭐ DataHandler.cs**

  서버와 데이터를 송 수신받는 Handler

  ---

- ✔️ **DateCheckHandler.cs**

  앱을 실행 한 상태로 하루(12시 00분 00초)가 지났을 때, 정원가기 페이지로 이동하는 기능

  (참고 : [이슈 #45 [DISCUSS] 00시 이후 DEFAULT 화면으로 이동](https://github.com/FluidTrack/MOA_garden/issues/45))

  ---

- **✔️ DeviceLog2.cs**

  수동 연결하는 과정에서 디바이스를 찾았을 때, 나타나는 기록들의 이름을 바꾸는 기능

  ---

- **✔️ DraggablePeeIcon.cs**

  꽃키우기 화면에서, 드래그 할 수 있는 오줌 아이콘 컨트롤

  ---

- ✔️ **DraggablePooIcon.cs**

  꽃키우기 화면에서, 드래그 할 수 있는 똥 아이콘 컨트롤

  ---

- **✔️ DraggableWaterIcon.cs**

  꽃키우기 화면에서, 드래그 할 수 있는 물 아이콘 컨트롤

  ---

- **✔️ EffectAnimatorAutoDelete.cs**

  애니메이션이 한 번 실행 된 다음, 자동으로 삭제가 될 필요가 있는 오브젝트들 (ex.이펙트)의 Handler

  ---

- ✔️ **KoreanUnderChecker.cs**

  한국어에서 이름 뒤에 붙는 조사(은/는) 또는 (이/가)를 판단하는 기능

  ---

- **✔️ MoabandReconnection.cs**

  모아밴드 수동연결을 담당하는 Handler

  ---

- **✔️ PlayOneShotAnimation.cs**

  애니메이션이 한 번 실행 된 다음, 자동으로 Disable될 필요가 될 필요가 있는 오브젝트들의 Handler

  ---

- **⭐ ProtocolHandler.cs**

  모아밴드에서 BLE를 통해 들어온 Byte Stream에 대한 Handler

  ---

- **⭐ SoundHandler.cs**

  모아밴드의 SFX또는 BGM을 조절하는 Handler

  ---

- **⭐ TimeHandler.cs**

  모아밴드에서 시간을 비교하는데 주로 사용되는 Handler

  ---

- **✔️ TimerHandler.cs**

  회원가입 페이지에서 시/분/초 입력을 제어하는 Handler

  ---

- **⛔ TimerHandler2.cs**

  회원가입 페이지에서 분/초 입력을 제어하는 Handler (이제 쓰이지 않음)

  ---

- **⭐ TotalManager.cs**

  처음 앱의 Init 과 각 페이지 Handler간 통신을 제어하는 메니저

  ---

- ✔️ **WateringAnimationSound.cs**

  꽃키우기 화면에서 물을 줄 때 효과음도 같이 나오게 하는 기능

