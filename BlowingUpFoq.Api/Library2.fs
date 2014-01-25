module Validation

type Result<'a, 'b> =
    | Success of 'a
    | Failure of 'b

let succeed = Success
let fail = Failure

let bind f = function
    | Success s -> f s
    | Failure f -> Failure f

let (>>=) input f =
    bind f input

let (>=>) f1 f2 =
    f1 >> bind f2

let switch f =
    f >> succeed

let tee f x =
    f x |> ignore; x

let tryCatch f onError x =
    try
        f x |> succeed
    with e -> e |> onError |> fail

let either onSuccess onFailure = function
    | Success s -> onSuccess s
    | Failure f -> onFailure f

let map f =
    either (f >> succeed) fail

let doubleMap onSuccess onFailure =
    either (onSuccess >> succeed) (onFailure >> fail)

let plus addSuccess addFailure f1 f2 x =
    match f1 x, f2 x with
    | Success s1, Success s2 -> addSuccess s1 s2 |> Success
    | Success _, Failure f2 -> Failure f2
    | Failure f1, Success _ -> Failure f1
    | Failure f1, Failure f2 -> addFailure f1 f2 |> Failure

let (&&&) v1 v2 =
    let addSuccess r1 r2 = r1
    let addFailure s1 s2 = s1 + "; " + s2
    plus addSuccess addFailure v1 v2