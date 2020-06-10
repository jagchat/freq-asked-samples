## Cookie Auth using ASP.NET Core 2.2 (without ASP.NET Core Identity)

 - Demonstrates on developing a basic ASP.NET Core 2.2 app with Cookie Authentication
 - The app does not use ASP.NET Core Identity or EF/Db and is a straight forward/basic way of integrating Cookie Authentication
 - App demonstrates the following:
 
 

### App demonstrates the following:
- Login with either "jag@email.com" (admin) or "scott@email.com" (member) with any password
- Logout functionality
- Using "Authorize" attribute with actions (and also with "Roles")
- Show claims of currently logged in user
- Try "/Home/ProtectedInfo" without log in.  Should redirect to /Security/Login automatically and back to "/Home/ProtectedInfo" once logged in.
- "admin" user will not be able to access "/Home/MemberInfo" (only the "Member" user will)
-- Shows AccessDenied page
- "member" user will have "MemberId" part of his claims and can be seen via "/Home/ProtectedInfo"
 
### Images

![01.png](images/01.png?raw=true "01.png")

![02.png](images/02.png?raw=true "02.png")

![03.png](images/03.png?raw=true "03.png")

![04.png](images/04.png?raw=true "04.png")

![05.png](images/05.png?raw=true "05.png")

![06.png](images/06.png?raw=true "06.png")

![07.png](images/07.png?raw=true "07.png")

![08.png](images/08.png?raw=true "08.png")

