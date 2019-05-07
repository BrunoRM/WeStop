angular.module('WeStop').controller('gameController', ['$routeParams', function ($routeParams) {

    let connection = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:5000/gameroom")
        .build();

        connection.on("connected", data => {
            console.log(data);
        });

    connection.start()
        .then(() => connection.invoke("join", {
            id: $routeParams.id,
            password: ''
        }));
    
}]);