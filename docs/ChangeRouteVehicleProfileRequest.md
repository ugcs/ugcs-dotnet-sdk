Changes route vehicle profile.

| Property     | Type | Description | Required |
|:------------- |:-------------|:----- |:-----|
| ClientId      | int | Current connection client ID | Yes 
| Route | Route | route object      | Yes 
| NewProfile | VehicleProfile | Vechile profile object | Yes 

```C#
ChangeRouteVehicleProfileRequest request = new ChangeRouteVehicleProfileRequest
{
    ClientId = _connect.AuthorizeHciResponse.ClientId,
    Route = route,
    NewProfile = new VehicleProfile { Id = vehicleProfile.Id }
};
```

ChangeRouteVehicleProfileResponse
```C#
var response = Executor.Submit<ChangeRouteVehicleProfileResponse>(request);
```