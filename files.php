<?php
$res=[];
$files=scandir('./');
foreach($files as $f){
	if($f=='.' || $f=='..' || $f=='files.php')continue;
	$res[]=$f;
}
echo implode('%123%',$res);
?>