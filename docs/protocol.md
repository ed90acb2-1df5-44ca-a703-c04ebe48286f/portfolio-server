# Protocol Documentation
<a name="top"></a>

## Table of Contents

- [authentication.proto](#authentication-proto)
    - [LoginRequest](#-LoginRequest)
    - [LoginResponse](#-LoginResponse)
    - [RegistrationRequest](#-RegistrationRequest)
    - [RegistrationResponse](#-RegistrationResponse)
  
- [errors.proto](#errors-proto)
    - [ErrorMessage](#-ErrorMessage)
  
    - [ErrorCode](#-ErrorCode)
  
- [messages.proto](#messages-proto)
    - [BroadcastMessage](#-BroadcastMessage)
  
- [requests.proto](#requests-proto)
    - [InputRequest](#-InputRequest)
  
- [values.proto](#values-proto)
    - [Vector2](#-Vector2)
  
- [Scalar Value Types](#scalar-value-types)



<a name="authentication-proto"></a>
<p align="right"><a href="#top">Top</a></p>

## authentication.proto



<a name="-LoginRequest"></a>

### LoginRequest



| Field | Type | Label | Description |
| ----- | ---- | ----- | ----------- |
| Login | [string](#string) |  |  |
| Password | [string](#string) |  |  |






<a name="-LoginResponse"></a>

### LoginResponse



| Field | Type | Label | Description |
| ----- | ---- | ----- | ----------- |
| Token | [string](#string) |  |  |
| ErrorCode | [ErrorCode](#ErrorCode) |  |  |






<a name="-RegistrationRequest"></a>

### RegistrationRequest



| Field | Type | Label | Description |
| ----- | ---- | ----- | ----------- |
| Login | [string](#string) |  |  |
| Password | [string](#string) |  |  |






<a name="-RegistrationResponse"></a>

### RegistrationResponse



| Field | Type | Label | Description |
| ----- | ---- | ----- | ----------- |
| ErrorCode | [ErrorCode](#ErrorCode) |  |  |





 

 

 

 



<a name="errors-proto"></a>
<p align="right"><a href="#top">Top</a></p>

## errors.proto



<a name="-ErrorMessage"></a>

### ErrorMessage



| Field | Type | Label | Description |
| ----- | ---- | ----- | ----------- |
| Code | [ErrorCode](#ErrorCode) |  |  |
| Description | [string](#string) |  |  |





 


<a name="-ErrorCode"></a>

### ErrorCode


| Name | Number | Description |
| ---- | ------ | ----------- |
| Success | 0 |  |
| RegistrationLoginExists | 1 |  |
| AuthenticationInvalidCredentials | 2 |  |


 

 

 



<a name="messages-proto"></a>
<p align="right"><a href="#top">Top</a></p>

## messages.proto



<a name="-BroadcastMessage"></a>

### BroadcastMessage



| Field | Type | Label | Description |
| ----- | ---- | ----- | ----------- |
| Positions | [Vector2](#Vector2) | repeated |  |





 

 

 

 



<a name="requests-proto"></a>
<p align="right"><a href="#top">Top</a></p>

## requests.proto



<a name="-InputRequest"></a>

### InputRequest



| Field | Type | Label | Description |
| ----- | ---- | ----- | ----------- |
| Direction | [Vector2](#Vector2) |  |  |





 

 

 

 



<a name="values-proto"></a>
<p align="right"><a href="#top">Top</a></p>

## values.proto



<a name="-Vector2"></a>

### Vector2



| Field | Type | Label | Description |
| ----- | ---- | ----- | ----------- |
| X | [float](#float) |  |  |
| Y | [float](#float) |  |  |





 

 

 

 



## Scalar Value Types

| .proto Type | Notes | C++ | Java | Python | Go | C# | PHP | Ruby |
| ----------- | ----- | --- | ---- | ------ | -- | -- | --- | ---- |
| <a name="double" /> double |  | double | double | float | float64 | double | float | Float |
| <a name="float" /> float |  | float | float | float | float32 | float | float | Float |
| <a name="int32" /> int32 | Uses variable-length encoding. Inefficient for encoding negative numbers – if your field is likely to have negative values, use sint32 instead. | int32 | int | int | int32 | int | integer | Bignum or Fixnum (as required) |
| <a name="int64" /> int64 | Uses variable-length encoding. Inefficient for encoding negative numbers – if your field is likely to have negative values, use sint64 instead. | int64 | long | int/long | int64 | long | integer/string | Bignum |
| <a name="uint32" /> uint32 | Uses variable-length encoding. | uint32 | int | int/long | uint32 | uint | integer | Bignum or Fixnum (as required) |
| <a name="uint64" /> uint64 | Uses variable-length encoding. | uint64 | long | int/long | uint64 | ulong | integer/string | Bignum or Fixnum (as required) |
| <a name="sint32" /> sint32 | Uses variable-length encoding. Signed int value. These more efficiently encode negative numbers than regular int32s. | int32 | int | int | int32 | int | integer | Bignum or Fixnum (as required) |
| <a name="sint64" /> sint64 | Uses variable-length encoding. Signed int value. These more efficiently encode negative numbers than regular int64s. | int64 | long | int/long | int64 | long | integer/string | Bignum |
| <a name="fixed32" /> fixed32 | Always four bytes. More efficient than uint32 if values are often greater than 2^28. | uint32 | int | int | uint32 | uint | integer | Bignum or Fixnum (as required) |
| <a name="fixed64" /> fixed64 | Always eight bytes. More efficient than uint64 if values are often greater than 2^56. | uint64 | long | int/long | uint64 | ulong | integer/string | Bignum |
| <a name="sfixed32" /> sfixed32 | Always four bytes. | int32 | int | int | int32 | int | integer | Bignum or Fixnum (as required) |
| <a name="sfixed64" /> sfixed64 | Always eight bytes. | int64 | long | int/long | int64 | long | integer/string | Bignum |
| <a name="bool" /> bool |  | bool | boolean | boolean | bool | bool | boolean | TrueClass/FalseClass |
| <a name="string" /> string | A string must always contain UTF-8 encoded or 7-bit ASCII text. | string | String | str/unicode | string | string | string | String (UTF-8) |
| <a name="bytes" /> bytes | May contain any arbitrary sequence of bytes. | string | ByteString | str | []byte | ByteString | string | String (ASCII-8BIT) |

