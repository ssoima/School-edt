(function () {
    angular.module('executionEngine')
        .factory('Engine', EngineService);

    EngineService.$inject = ['$rootScope', '$http', 'baseApi'];

    function EngineService($rootScope, $http, baseApi) {
        var services = {
            get: get,
            createNew: createFake,
            enqueue: enqueue
        };
        return services;

        function get() {
            return $http({
                url: baseApi + '/orders'
            });
        }

        function createFake(customerName) {
            var color = getRandomColor();
            return $http({
                url: baseApi + '/orders/createfake',
                method: 'POST',
                data: {
                    customer: customerName,
                    variant: color
                }
            });
        }

        function enqueue(order) {
            return $http({
                url: baseApi + '/orders/enqueue',
                method: 'POST',
                data: {
                    order: order.no
                }
            });
        }

        function getRandomColor() {
            var colors = [
                'black',
                'red',
                'white',
                'blue',
                'purple',
                'yellow',
                'pink',
                'grey',
                'crystal-blue',
                'crystal-white',
                'green',
                'neon-green'
            ];
            return colors[Math.floor((Math.random() * colors.length) + 1) % (colors.length)];
        }
    }
})();