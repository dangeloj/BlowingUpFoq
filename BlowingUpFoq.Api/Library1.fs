module Accounts

open System
open Simple.Web
open Simple.Web.Authentication
open Simple.Web.Behaviors
open UserData
open Validation

let notNull errorMessage x =
    if obj.ReferenceEquals(null, x) then errorMessage |> Failure
    else x |> Success

module Login =
    type LoginModel = { Email: string; Password: string }

    [<AutoOpen>]
    module private Helpers =
        let getUser (service:IUserService) model =
            service.AuthenticateUser model.Email model.Password |> function
            | Some r ->
                match r with
                | Authenticated(id) ->
                    User(id, model.Email) :> IUser |> succeed
                | _ -> failwith "Error"
            | None -> failwith "Error"

        let validateAndAuthenticate service =
            notNull "Email address and password are required"
            >=> getUser service

    [<UriTemplate("/api/account/login")>]
    type LoginPost(service:IUserService) as this =
        let validateAndAuthenticate = validateAndAuthenticate service
        member val LoggedInUser:IUser = null with get, set

        interface IPost<LoginModel> with
            member __.Post model =
                match validateAndAuthenticate model with
                | Success s -> failwith "Error"
                | Failure f -> Status(400, f)

        interface ILogin with
            member __.LoggedInUser with get() = this.LoggedInUser