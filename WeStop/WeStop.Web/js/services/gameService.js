angular.module('WeStop').factory('$game', ['$rootScope', '$q', function ($rootScope, $q) {

    var connection = null;

    function connect () {

        let deferred = $q.defer();

        if (connection) {
            deferred.resolve();
            return deferred.promise;
        }

        connection = new signalR.HubConnectionBuilder()
            .withUrl("http://localhost:5000/gameroom")
            .build();

        connection.onclose(function () {
            console.log('connection closed');
        });

        connection.start().then(() => { 
            $rootScope.$apply(() => deferred.resolve());
        }, (e) => { 
            $rootScope.$apply(() => deferred.reject(e));
        });

        return deferred.promise;
    }

    function on(eventName) {

        let deferred = $q.defer();

        if (!connection) deferred.reject('Não existe nenhuma conexão estabelecida com o servidor');
        
        connection.on(eventName, data => {
            $rootScope.$apply(() => {
                deferred.resolve(data);
            });
        });
        
        return deferred.promise;
    }

    function invoke(eventName, params) {

        // let deferred = $q.defer();
        if (!connection) deferred.reject('Não existe nenhuma conexão estabelecida com o servidor');

        if (params)
            connection.invoke(eventName, params);
        else
            connection.invoke(eventName);

        // return deferred.promise;
    }

    return {
        connect: connect,
        on: on,
        invoke: invoke
    }


}]);