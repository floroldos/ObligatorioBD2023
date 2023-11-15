/*CREATE DATABASE ObligatorioBD2023;*/
USE ObligatorioBD2023;

CREATE TABLE Logins(
    LogId INT PRIMARY KEY ,
    Password VARCHAR(50) NOT NULL
);

CREATE TABLE Funcionarios(
    Ci INT(8) PRIMARY KEY ,
    Nombre VARCHAR(50) NOT NULL ,
    Apellido VARCHAR(50) NOT NULL ,
    Fch_Nacimiento DATE NOT NULL ,
    Direccion VARCHAR(100) NOT NULL ,
    Telefono INT NOT NULL ,
    Email VARCHAR(100) NOT NULL ,
    LogId INT NOT NULL ,
    FOREIGN KEY (LogId) REFERENCES Logins(LogId)
);

CREATE TABLE Agenda(
    Nro INT PRIMARY KEY AUTO_INCREMENT ,
    Ci INT(8) NOT NULL ,
    Fch_Agenda DATE NOT NULL
);

CREATE TABLE Carnet_Salud(
    Ci INT(8) PRIMARY KEY ,
    Fch_Emision DATE NOT NULL ,
    Fch_Vencimiento DATE NOT NULL ,
    Comprobante VARCHAR(200) NOT NULL ,
    FOREIGN KEY (Ci) REFERENCES Funcionarios(Ci)
);

CREATE TABLE Periodos_Actualizacion(
    Año YEAR NOT NULL ,
    Semestre VARCHAR(20) ,
    Fch_Inicio DATE PRIMARY KEY ,
    Fch_Fin DATE NOT NULL
);