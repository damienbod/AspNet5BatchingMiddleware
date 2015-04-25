(function () {
	'use strict';

	function TestClassOneService($http, $log) {

		$log.info("TestClassOneService called");

		var getAnigetAllmals = function () {
			$log.info("TestClassOneController getAll called");
			return $http.get("/api/TestClassOneController")
			.then(function (response) {
				return response.data;
			});
		}

		return {
			getAll: GetAll
		}
	}

	var module = angular.module('myapp');

    module.factory("TestClassOneService",
		[
			"$http",
			"$log",
			TestClassOneService
		]
	);
})();