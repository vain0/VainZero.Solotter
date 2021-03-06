﻿namespace VainZero.Solotter.Desktop

open System
open System.Reactive.Linq
open System.Windows.Input
open Reactive.Bindings
open VainZero.Solotter

[<Sealed>]
type UserAuthPage(accessToken: AppAccessToken, notifier: Notifier) =
  let twitterCred =
    let t = accessToken
    Tweetinvi.Models.TwitterCredentials(t.ConsumerKey, t.ConsumerSecret)

  let authContext =
    Tweetinvi.AuthFlow.InitAuthentication(twitterCred)

  let getPinCodeCommand =
    RaisableCommand.create
      (fun () -> true)
      (fun () ->
        let url = authContext.AuthorizationURL
        System.Diagnostics.Process.Start(url) |> ignore
      )

  let pinCode =
    new ReactiveProperty<string>("")

  let authenticateCommand =
    pinCode.Select(String.IsNullOrEmpty >> not).ToReactiveCommand()

  let authenticate () =
    let pinCode = pinCode.Value
    let cred =
      Tweetinvi.AuthFlow.CreateCredentialsFromVerifierCode(pinCode, authContext)
    if cred |> isNull then
      notifier.NotifyInfo("Incorrect PinCode.")
      Observable.Never()
    else
      let userAccessToken =
        {
          AccessToken =
            cred.AccessToken
          AccessSecret =
            cred.AccessTokenSecret
        }
      Observable.Return(CompleteAuth (accessToken, userAccessToken))

  let authenticated =
    authenticateCommand
      .SelectMany(fun _ -> authenticate ())
      .Replay()

  let subscription =
    authenticated.Connect()

  let dispose () =
    subscription.Dispose()
    pinCode.Dispose()

  member this.GetPinCodeCommand =
    getPinCodeCommand

  member this.PinCode =
    pinCode

  member this.AuthenticateCommand =
    authenticateCommand :> ICommand

  member this.Authenticated =
    authenticated :> IObservable<_>

  member this.Dispose() =
    dispose ()

  interface IObservable<AuthState> with
    override this.Subscribe(observer) =
      authenticated.Subscribe(observer)

  interface IDisposable with
    override this.Dispose() =
      this.Dispose()

  interface IAuthPage
