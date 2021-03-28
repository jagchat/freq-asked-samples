## Google Auth using ASP.NET Core 2.2 (without ASP.NET Core Identity)

 - Demonstrates on developing a basic ASP.NET Core 2.2 app with Google Authentication integration
 - The app does not use ASP.NET Core Identity or Db and is a straight forward/basic way of integrating Google Authentication (OAuth)

### Steps to follow
- Open Google API Console (and login)
-- go to https://console.developers.google.com/apis/credentials
- Select **Credentials** | **OAuth Client Id**
- Select App Type: **Web Application**, Name: **sample** and hit **Create**
- specify "Authrozed redirect URIs" and hit **Save**:
--https://localhost:44359/signin-google
- Note down generated Client Id/Secret and use it in **Startup.cs**
### Images

![00.png](images/00.png?raw=true "00.png")

![01.png](images/01.png?raw=true "01.png")

![02.png](images/02.png?raw=true "02.png")

![03.png](images/03.png?raw=true "03.png")