<?php
if(isset($_FILES['crashdmp']) && $_FILES['crashdmp']['error'] == 0){
	$arrType = array('application/x-dmp');
	$arrExt = array('dmp');
	
	// Получаем расширение файла
	$ext = pathinfo($_FILES['crashdmp']['name'], PATHINFO_EXTENSION);
	// Проверка MIME-тип файла и расширение
	$finfo = new finfo(FILEINFO_MIME_TYPE);
	$type = $finfo->file($_FILES['crashdmp']['tmp_name']);
	//echo($ext.' '.$type);
	if (in_array($type, $arrType) && in_array($ext, $arrExt)){
		$filepath = 'путь/skymp/uploads/hidden/crashdmps/'.md5_file($_FILES['crashdmp']['tmp_name']).'.'.$ext;
		if(move_uploaded_file($_FILES['crashdmp']['tmp_name'], $filepath)){
			exit("OK");            
		} else {
			exit('eUpload');
		}
	}
	exit('eNotSupFile');
}
exit('eNoFile');