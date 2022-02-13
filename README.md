# SplinterBot 
A simple solution to help mange a multiple accounts in Splinterlands. 
The Splinterbots can help you with: 
 * Sending DEC to one main account
 * Claim SPS 
 * Sending SPS to one main account 
 * Automatically rent card to ensure that the bronze 2 league 
 * Nice visual configuration for settings and accounts
 * (IN future) Visual statistics both in app and on the website
 * (In future) ability to rent cards from predefined list
 * (In future) ability to buy cards daily to get power in the long run.
 * (IN future) a battles and tournament playing functionality

There is no subscription for the bot, it is taking 5% of DEC and SPS on every transfer.

This bot is still work in progress, and as such some things might change. 
I have an idea to improve thte UI and onboarding process but it depends on the free time I will get.

# Installation 
Download bot from [Releases page](https://github.com/functional-solutions/SplinterlandClaimBot/releases) it will contain the single executable file with config. Just  run the exe file on your machine.

# Configuratin
Currently there is not UI configuration so all change have to be done in config.yml and account.yml. Idea is that config.yml contains the overridable settings which cabn change with every release and *accounts.yml* contains the list of accounts. 

Create the **account.ym** file like: 
```
sentTo: 'main_account_name'  
accounts:
  - username: 'account1_name'
    postingKey: 'account1_posting_key'
    activeKey: 'account1_active_or_master_key'
  - username: 'account2_name'
    postingKey: 'account2_posting_key'
    activeKey: 'account2_active_or_master_key'
```

The option in coinfig.yml can be changed as well but the default should be fine

# Screenshotts 

![bot_dashboard](https://user-images.githubusercontent.com/396409/139722666-650eadda-c3d8-49bb-8183-67a26129735e.png)

# Donation 
Event the bot it taking some donations, there is an option to help me more:
 * DEC in game in account @splinterbots 
 * BNB/ETH/MATIC 0x9761a8520ae18EA851544A17905256D0c3AEc688

# Discord 
[Discord](https://discord.gg/C4Y2CBaxY8)

# Build status
[![Build Status](https://dev.azure.com/be-functional/Splinterbots/_apis/build/status/functional-solutions.Splinterbots?branchName=master)](https://dev.azure.com/be-functional/Splinterbots/_build/latest?definitionId=47&branchName=master)
