# cqrs-dot-net-core
## cqrs dot net core 2.1
## redis https://github.com/MicrosoftArchive/redis

## Pull code and build project IotHub.ConsoleSample
## Run redis first 
On window https://github.com/badpaybad/cqrs-dot-net-core/blob/master/Redis28.zip download -> extract -> redis-server.exe
## Run project IotHub.ConsoleSample -> show up command window -> type command pubsub or pubsubmad to test


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

		
## Can build api and send command, see how it run in console
Run project IotHub.EcommerceApi you will got eg: https://localhost:5001/swagger/index.html?url=/swagger/v1/swagger.json#/CommandSender

Sample data to post to /api/CommandSender,  should modify PublishedCommandId, it should be unique
				{
				"CommandTypeFullName":"IotHub.CommandsEvents.SampleDomain.CreateSample",
				"CommandDataJson":'{"PublishedCommandId":"ac349cdc-5b50-4d87-b8fe-296d67745eae","Version":0,"SampleVersion":"10/12/2018 6:44:29 AM", "TokenSession":"",
				"SampleId":"ac349cdc-5b50-4d87-b8fe-296d67745eae"}'
				}
