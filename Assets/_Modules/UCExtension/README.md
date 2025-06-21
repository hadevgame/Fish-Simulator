
## Unity Common Extension




## Installation
- Tạo SSH bằng CMD (Không cần nhập password).
> ssh-keygen -t rsa -b 2048 -C "<comment>"
- Truy cập vào file ***id_rsa.pub*** để lấy public key.
> ![](https://lh3.googleusercontent.com/d/1FxWD15-XfbX8w6oyE2IwtD4I9U6a6K3h)
- Vào gitlab chọn ***"Avatar >> Preference >> SSH Keys"*** để tạo SSH key Bằng key vừa lấy được.
- Trong Fork chọn  ***"Submodule >> Add Submodule"*** để thêm submodule vào dự án.
> git@gitlab.com:luongnd28200/unitycommonextension.git
> Assets/UCExtension
> ![](https://lh3.googleusercontent.com/d/1Gsd4s5YZMtreUpgciNVugfmjnDxxHdaW)
## GUI
#### BaseGUI
- BaseGUI là class quản lý một nhóm UI trong game như: Game Play, Loading, Home, Settings, Shop,...
```
protected virtual void SetUp()
public virtual void Open()
public virtual void Close()
public void OnClose(Action callback)
public virtual void OnOpened()
public virtual void OnClosed()
```
- Thành phần của một GUI:
> ![](https://lh3.googleusercontent.com/d/1m9xNHjFqX3WsBVOSFN4tSkzF0wrQaJv0)
#### GUIController
- GUIController có nhiệm vụ "Instantiate, Open, Close" các GUI
- GUIController sẽ Load GUI mới từ Resources
> Resources/GUI/[GUIName]
- Khi tạo GUI mới cần đưa vào đúng thư mục và tên GUI (GUI sẽ có button sửa tên trên Editor)