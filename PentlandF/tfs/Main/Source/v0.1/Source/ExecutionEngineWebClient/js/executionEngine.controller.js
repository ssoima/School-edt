(function () {
    angular.module('executionEngine')
        .controller('ExecutionController', ExectuionController);

    ExectuionController.$inject = ['$scope', 'Engine'];

    function ExectuionController($scope, Engine) {
        $scope.orders = [];
        $scope.createNewOrder = createFake;
        $scope.queueChanged = queueChanged;
        $scope.loadingComplete = false;
        getOrders();

        function getOrders() {
            Engine.get().then(onGetOrdersSucceeded);
        }

        function createFake() {
            Engine.createNew('Lukas').then(onCreateFakeSucceeded);
        }

        function queueChanged(order) {
            Engine.enqueue(order).then(onEnqueueSucceeded);
        }

        function onCreateFakeSucceeded(data) {
            $scope.orders.push(enrichOrder(data.data));
        }

        function onGetOrdersSucceeded(data) {
            $scope.orders = angular.forEach(data.data, enrichOrder);
            $scope.loadingComplete = true;
        }

        function onEnqueueSucceeded(data) {
            // find order
            var orderIdx = _.findIndex($scope.orders, function (item) { return item.no == data.data.no; });
            var updatedOrder = enrichOrder(data.data);
            $scope.orders[orderIdx] = updatedOrder;
        }

        function enrichOrder(order) {
            order.isQueued = order.status != 'New';
            return order;
        }
    }
})();