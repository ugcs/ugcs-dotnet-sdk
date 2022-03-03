Send login request

| Property     | Type | Description | Required |
|:------------- |:-------------|:----- |:-----|
| ClientId      | int | Current connection client ID | Yes 
| UserLogin | string | User login      | Yes 
| UserPassword | string | user password      | Yes 

Example:
```c#
LoginRequest loginRequest = new LoginRequest();
loginRequest.UserLogin = "Login";
loginRequest.UserPassword = "Password";
loginRequest.ClientId = clientId;
```
LoginResponse
```c#
var loginResponce = _executor.Submit<LoginResponse>(loginRequest);
```