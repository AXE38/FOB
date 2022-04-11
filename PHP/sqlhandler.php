<?php
	Class sqlhandler {
		private $sName;
		private $sConn;

		function init() {
			$xml = simplexml_load_file('config.xml');
			$this->sName = (string)$xml->ServerName;
			$DB = (string)$xml->Database;
			$UID = (string)$xml->UID;
			$PWD = (string)$xml->PWD;
			$this->sConn = array('Database' => $DB,
					   'UID' => $UID,
					   'PWD' => $PWD,
					   'CharacterSet' => 'UTF-8');
			var_dump($this->sConn);
		}
		function sql_exec($sql, $params = '') {
			$conn = sqlsrv_connect($this->sName, $this->sConn);
			$stmt = sqlsrv_query($conn, $sql, $params);
			if($stmt === false ) {
			    if (sqlsrv_errors() != null) {
			    		//sqlsrv_close($conn);
			            return sqlsrv_errors();
			    }
			}
			$result = array();
			while( $row = sqlsrv_fetch_array( $stmt, SQLSRV_FETCH_ASSOC) ) {
				array_push($result, $row);
			}
			sqlsrv_close($conn);
			return $result;
		}
	}
?>