(function () {
	'use strict';

	function TestClassTwoService($http, $log) {

		$log.info("TestClassTwoService called");

		var getAll = function () {
			$log.info("TestClassTwoController getAll called");
			return $http.get("http://localhost:13605/api/TestClassTwo")
			.then(function (response) {
				return response.data;
			});
		}

		return {
			getAll: getAll
		}
	}

	var module = angular.module('myapp');

	module.factory("TestClassTwoService",
		[
			"$http",
			"$log",
			TestClassTwoService
		]
	);
})();