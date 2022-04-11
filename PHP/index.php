<?php
	include 'sqlhandler.php';
	$sql = new SqlHandler();
	$sql->init();

	function getError($errorcode) {
		$xml = new simplexmlElement('<root/>');
		$node1 = $xml->addChild('state', $errorcode);
		return $xml->asXML();
	}

	function getColumns($type, $entity) {
		$columns = "";
		$values = "";
		foreach ($entity->children() as $key => $val) {
			switch (trim($type)) {
				case "INSERT":
					$columns = $columns . $key . ",";
					$values = $values . "'" . $val . "',";
					break;
				case "UPDATE":
					$values = $values . $key . "='" . $val . "',";
					break;
				case "SELECT":
					$columns = $columns . $key . ",";
					break;
			}
		}
		$columns = rtrim($columns, ",");
		$values = rtrim($values, ",");
		return array("columns"=>$columns, "values"=>$values);
	}

	function getWhere($where) {
		if ( !((bool)$where) || count($where->children()) == 0) {
			return " ";
		}
		$expr = " where ";
		foreach ($where->condition as $val) {
			if (trim($val->column) == "abs_password") {
				$val->value = hash("sha256", $val->value);
			}
			$expr = $expr . $val->column . " " . $val->operator . " '" . $val->value . "' AND ";
		}
		$expr = rtrim($expr, " AND ");
		return $expr;
	}

	function Auth($where) {
		global $sql;

		if (!((bool)$where)) {
			echo getError(-3);
			exit;
		}

		$operator = "select id, token from ABS_USER " . getWhere($where);
		//echo $operator;
		$res = $sql->sql_exec($operator, null);

		if (count($res) == 0) {
			echo getError(-3);
		} else {
			$res = $res[0];
			$id = $res['id'];
			$token = $res['token'];
			if ($token == "") {
				$token = hash("sha256", date(DATE_ATOM));
			}
			$dt = date('Y-m-d H:i:s');
			$operator = "update ABS_USER set last_login_date = '" . $dt . "', token = '" . $token . "' where id = " . $id;
			$res = $sql->sql_exec($operator, null);
			$xml = new simplexmlElement('<root/>');
			$node1 = $xml->addChild('state', '200');
			$node1 = $xml->addChild('token', $token);
			echo $xml->asXML();
		}
	}

	function CheckAuth($token) {
		global $sql;

		$operator = "select id, token from ABS_USER where token = '{$token}'";
		$res = $sql->sql_exec($operator, null);

		if (count($res) == 0) {
			echo getError(-3);
			exit;
		}
	}

	function getSql($type, $entity_type, $entity, $token, $where = "", $offset = "", $fetch = "") {
		switch (trim($type)) {
			case "INSERT":
				$arr = getColumns($type, $entity);
				$operator = "insert into " . $entity_type . " (id, " . $arr['columns'] . ") values (NEXT VALUE FOR dbo.MAIN_SEQ, " . $arr['values'] . ");";
				break;
			case "UPDATE":
				$arr = getColumns($type, $entity);
				$operator = "update " . $entity_type . " set " . $arr['values'] . " " . getWhere($where);
				break;
			case "SELECT":
				$arr = getColumns($type, $entity);
				$operator = "select " . $arr['columns'] . " from " . $entity_type . " " . getWhere($where);
				if ($offset != "" && $fetch != "") {
					$operator = $operator . " order by 1 offset {$offset} rows fetch next {$fetch} rows only";
				};
				break;
			case "DELETE":
				$operator = "delete from " . $entity_type . " " . getWhere($where);
				break;
			break;
		}
		$stmt = $operator . " ";
		//echo $operator;
		return $stmt;
	}

	function getXmlFromRes($res) {
		
		$xml = new simplexmlElement('<root/>');
		$node1 = $xml->addChild('state', '200');
		$node1 = $xml->addChild('data');
		foreach ($res as $row) {
			$node2 = $node1->addChild('row');
			foreach ($row as $key=>$val) {
				if ($val instanceof DateTime) {
					$val = $val->format('Y-m-d H:i:s');
				}
				$node3 = $node2->addChild($key, $val);
			}
		}
		echo $xml->asXML();
	}

	//echo(header('content-type: text/xml'));
	//echo(header('content-type: text'));
	libxml_clear_errors();
	libxml_use_internal_errors(TRUE);
	if (!isset($_POST['request'])) {
		echo getError(-1);
		exit;
	} elseif (isset($_POST['request'])) {
		$request = $_POST['request'];
	}
	
	$type = null;
	try {
		$xml = new simplexmlElement($request, LIBXML_NOWARNING | LIBXML_NOERROR);
		$type = $xml->type;
		$entity = $xml->entity;
		$entity_type = $xml->entity_type;
		$token = $xml->token;
		$where = $xml->where;
		$offset = $xml->offset;
		$fetch = $xml->fetch;
		/*echo "type = " . $type;
		echo "entity_type = " . $entity_type;
		echo "entity = " . $entity->asXML();
		echo "token = " . $token . PHP_EOL;
		echo "where = " . $where->asXML() . "\n";
		echo "offset = " . $offset . "\n";
		echo "fetch = " . $fetch . "\n";*/
	} catch (Exception $e) {
		echo getError(-2);
	} finally {
		libxml_clear_errors();
	}
	if (trim($type) == "AUTH") {
		Auth($where);
		exit;
	} else {
		CheckAuth(trim($token));
	}
	$stmt = getSql($type, $entity_type, $entity, $token, $where, $offset, $fetch);
	$res = $sql->sql_exec($stmt, null);
	//var_dump($res);
	echo getXmlFromRes($res);
	//echo getSql($type, $entity_type, $entity, $token, $where, $offset, $fetch);

	//echo hash("sha256", 'ADMIN');
?>