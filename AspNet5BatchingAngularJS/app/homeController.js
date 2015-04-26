(function () {
	'use strict';

	var module = angular.module('myapp');

	module.controller('homeController',
		[
			'$scope',
			'$http',
			'$log',
			'testClassOne',
			'testClassTwo',
			homeController
		]
	);

	function homeController($scope, $http, $log, testClassOne, testClassTwo) {
		$scope.testClassOne = testClassOne;
		$scope.testClassTwo = testClassTwo;
		console.log('home controller called');
	}
})();

