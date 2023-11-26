CREATE TABLE Logins(
    LogId INT AUTO_INCREMENT PRIMARY KEY ,
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
    Nro INT AUTO_INCREMENT PRIMARY KEY,
    Ci INT(8) ,
    Fch_Agenda DATE NOT NULL
);

CREATE TABLE Carnet_Salud(
    Ci INT(8) PRIMARY KEY ,
    Fch_Emision DATE NOT NULL ,
    Fch_Vencimiento DATE NOT NULL ,
    Comprobante VARCHAR(350) NOT NULL ,
    FOREIGN KEY (Ci) REFERENCES Funcionarios(Ci)
);

CREATE TABLE Periodos_Actualizacion(
    Anio YEAR NOT NULL ,
    Semestre VARCHAR(20) ,
    Fch_Inicio DATE PRIMARY KEY ,
    Fch_Fin DATE NOT NULL
);

CREATE TABLE Rol(
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Rol VARCHAR(30) NOT NULL
);
INSERT INTO Rol(Rol) VALUES('Admin');
INSERT INTO Rol(Rol) VALUES('Funcionario');

CREATE TABLE UsuarioRol(
    Ci_Funcionario INT NOT NULL ,
    Id_Rol INT NOT NULL ,
    PRIMARY KEY (Ci_Funcionario, Id_Rol) ,
    FOREIGN KEY (Ci_Funcionario) REFERENCES Funcionarios(Ci) ,
    FOREIGN KEY (Id_Rol) REFERENCES Rol(Id)
);

SELECT LAST_INSERT_ID()