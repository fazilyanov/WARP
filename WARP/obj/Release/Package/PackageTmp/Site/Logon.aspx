<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Logon.aspx.cs" Inherits="WARP.Logon" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta name="description" content="">
    <meta name="author" content="">
    <link rel="icon" href="/Content/favicon.ico" type="image/ico" />
    

    <title>Авторизация</title>

    <!-- Bootstrap core CSS -->
    <link href="../Content/bootstrap-3.3.6/css/bootstrap.min.css" rel="stylesheet" />

    <style>
        body {
            padding-top: 40px;
            padding-bottom: 40px;
            background-color: #eee;
        }

        .form-signin {
            max-width: 330px;
            padding: 15px;
            margin: 0 auto;
        }

            .form-signin .form-signin-heading,
            .form-signin .checkbox {
                margin-bottom: 10px;
            }

            .form-signin .checkbox {
                font-weight: normal;
            }

            .form-signin .form-control {
                position: relative;
                height: auto;
                -webkit-box-sizing: border-box;
                -moz-box-sizing: border-box;
                box-sizing: border-box;
                padding: 10px;
                font-size: 16px;
            }

                .form-signin .form-control:focus {
                    z-index: 2;
                }

            .form-signin input[type="email"] {
                margin-bottom: -1px;
                border-bottom-right-radius: 0;
                border-bottom-left-radius: 0;
            }

            .form-signin input[type="password"] {
                margin-bottom: 10px;
                border-top-left-radius: 0;
                border-top-right-radius: 0;
            }
    </style>
</head>

<body>

    <div class="container">

        <form class="form-signin" method="POST" action="Logon.aspx">
            <h2 class="form-signin-heading">Авторизуйтесь</h2>
            <span style="color:red;"><%=error%></span>
            <label for="inputEmail" class="sr-only">Логин</label>
            <input type="text" id="login" name="login" class="form-control" placeholder="Логин" value="<%=login%>" required autofocus>
            <label for="inputPassword" class="sr-only">Пароль</label>
            <input type="password" id="password" name="password" class="form-control" placeholder="Пароль" value="<%=password%>" required>
            <div class="checkbox">
                <label>
                    <input type="checkbox" name="rememberme" value="1">
                    Запомнить
                </label>
            </div>
            <button class="btn btn-lg btn-primary btn-block" type="submit">Войти</button>
        </form>
    </div>
</body>
</html>