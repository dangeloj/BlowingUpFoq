module UserData

open System
open Crypto

type AuthenticationResult =
    | Authenticated of Guid
    | Unauthenticated

type IUserService =
    abstract EmailExists: emailAddress:string -> bool
    abstract CreateAccount: emailAddress:string -> password:string -> Guid option
    abstract AuthenticateUser: emailAddress:string -> password:string -> AuthenticationResult option