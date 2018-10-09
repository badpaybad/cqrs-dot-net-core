# cqrs-dot-net-core
cqrs dot net core 2.1

Code structure
https://drive.google.com/open?id=15n6s2WaxVN33BTBop0XXjjlxlYHuMz_u

+ Apis // receive request from client and convert to command to send, or do query facade return to client
|-IotHub.EcommerceApi // can using IotHub.Core to bootup Domains to run as single web app
|-IotHub.OAuth
+ Core
|-IotHub.CommandsEvents //all command and event will declare here to share whole project
|-IotHub.Core // infrastructure for cqrs enginee and event sourcing, share whole project
+ Domains
|- // Your domain business project, can use aggregateroot to do DDD or just only hanlde to do business
+ EngineeBackgroundJobs // run able to boot up Domains to run business
|- // Can be console, or background services eg: window services
|- // Can bootup domain to do distributed process as microservices
+ Test
|- // Do unit test for whole solution
+ Uis
|- // js framework to make ui for client and connect to Apis. eg: Angular, Reactjs, Veujs
