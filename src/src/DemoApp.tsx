import { Routes, Route, HashRouter } from "react-router-dom";
import React from 'react'
import Editor from "./pages/Editor";
import Home from "./pages/Home";
import NotFound from "./pages/NotFound";
function DemoApp() {
  return (
    <HashRouter>
      <Routes>
      <Route path="/home" element={<Home />} />
      <Route path="/detail/:id" element={<Editor />} />
      <Route path="*" element={<NotFound />} />
      </Routes>
    </HashRouter>    
  );
}

export default DemoApp;
