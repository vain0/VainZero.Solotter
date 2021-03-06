﻿namespace VainZero.Solotter

  open System.Runtime.Serialization
  open Prism.Commands
  open System.IO

  type RaisableCommand<'TParameter> =
    DelegateCommand<'TParameter>

  [<AbstractClass>]
  type Notifier() =
    abstract NotifyInfo: string -> unit
    abstract Confirm: string -> bool

  [<DataContract>]
  type AppAccessToken =
    {
      [<field: DataMember>]
      ConsumerKey:
        string
      [<field: DataMember>]
      ConsumerSecret:
        string
    }

  [<DataContract>]
  type UserAccessToken =
    {
      [<field: DataMember>]
      AccessToken:
        string
      [<field: DataMember>]
      AccessSecret:
        string
    }

  [<DataContract>]
  type AccessToken =
    {
      [<field: DataMember>]
      AppAccessToken:
        option<AppAccessToken>
      [<field: DataMember>]
      UserAccessToken:
        option<UserAccessToken>
    }

  type Auth =
    {
      AppAccessToken:
        AppAccessToken
      UserAccessToken:
        UserAccessToken
      Twitter:
        Tweetinvi.Models.ITwitterCredentials
    }

  type IConfigShape<'TConfig> =
    abstract IsPortable: bool
    abstract Empty: 'TConfig

    abstract ReadFromStream: Stream -> 'TConfig
    abstract WriteToStream: 'TConfig * Stream -> unit

  type IConfigRepo<'TConfig> =
    abstract Find: unit -> 'TConfig
    abstract Save: 'TConfig -> unit

  type AccessTokenRepo =
    IConfigRepo<AccessToken>
