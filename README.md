# Personal Finance Manager
### ⚠️ Proyecto en proceso ⚠️

Este proyecto se encuentra actualmente en desarrollo activo. Las funcionalidades se encuentran implementadas y estan en proceso de testeo.

## 📌 Descripción general

Aplicación web de gestión de gastos personales, orientada a usuarios que desean registrar, categorizar, filtrar y analizar sus gastos mediante tablas y gráficos, con posibilidad de exportar la información.

El objetivo del proyecto es servir tanto como herramienta práctica real como proyecto demostrativo de buenas prácticas en backend y frontend.

## ✅ Funcionalidades
### 🔐 Autenticación y usuarios

Registro de usuarios (De forma local o con servicios de terceros)

Inicio de sesión (De forma local o con servicios de terceros)

Cierre de sesión

Protección de rutas por usuario

### 📂 Categorías

Crear categorías

Editar categorías

Eliminar categorías (borrado lógico si tiene gastos asociados)

Listado de categorías por usuario

### 💸 Gastos / Expensas

Crear gastos

Editar gastos

Eliminar gastos

Listado de gastos por usuario

Asociación de gastos a categorías

### 🔍 Filtros

Filtrado por rango de fechas (desde / hasta)

Filtrado por rango de valores (minimo / maximo)

Filtrado por categoría

Combinación de filtros

### 📊 Dashboard y gráficos

Gráfico de gastos por categoría (implementado)

Gráficos mensuales (implementado)

Gráficos anuales (implementado)

Múltiples gráficos en dashboard (implementado)

### 📈 Visualización de datos

Tablas HTML con Bootstrap

### 📤 Exportación de datos

Exportar gastos a Excel (implementado)

Exportación diaria (implementado)

Exportación mensual (implementado)

Exportación anual (implementado)

Exportación total (implementado)

### 🧱 Arquitectura
Modelo-Vista-Controlador (MVC)

Separación en capas (Controller / Service / Repository)

Uso de DTOs y ViewModels

Validaciones centralizadas

Acceso a datos mediante Entity Framework Core

## 🛠️ Tecnologías utilizadas
### Backend

C#

ASP.NET Core MVC

Entity Framework Core

PostgreSQL

Identity

ClosedXML

Localización (Inglés y Español con IStringLocalizer)

### Frontend

Razor Views

HTML5

CSS3

Bootstrap

JavaScript

Chart.js

#### Otros

Git / GitHub

## 🚧 Estado del proyecto

El proyecto está en desarrollo. Las próximas etapas incluyen:

Completar todos los gráficos del dashboard

Mejorar la visualización de tablas

Implementar exportación a Excel

Optimizar UX/UI

## 📄 Licencia

Proyecto personal con fines educativos y profesionales.
