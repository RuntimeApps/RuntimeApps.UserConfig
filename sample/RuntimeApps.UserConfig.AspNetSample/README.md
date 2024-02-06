# Asp.Net sample of RuntimeApps user config

This example showes how to use RuntimeApps.UserConfig in Asp.Net.

There is two roles:
* `User` role which can view and edit own config.
* `Admin` role which can edit default configs too.

A default Admin user has been added to the sample with user name `Admin` and password of `@Admin123`.

Some keys have been added:
* Key-Changeable: This key can be costomized by any user.
* Key-Admin-Change: This key can be costomized by Admin.
* Key-Not-Changeable: this key can not be changed by anyone and is readonly.
