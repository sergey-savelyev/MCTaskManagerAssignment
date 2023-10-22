<header class="text-center">
    <h1>ToDoList</h1>
    <h4 id="subTitle">Test assignment completed by Sergei Savelev</h4>
</header>
  
<div class="container mt-4">
    <ul class="nav nav-pills justify-content-center">
      <li class="nav-item">
        <a class="nav-link" href="/tasks">Tasks</a>
      </li>
      <li class="nav-item">
        <a class="nav-link" href="/logs">Logs</a>
      </li>
    </ul>
    <div>
        <slot />
    </div>
</div>

<style>
header {
    padding: 20px;
}
#subTitle {
    font-size: 14px;
    color: #999;
}
</style>