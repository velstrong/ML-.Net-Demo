/**
 * @file
 * @date 2024-08-01
 * @author haodong.wang
 * @lastModify  2024-08-01
 */
/* <------------------------------------ **** DEPENDENCE IMPORT START **** ------------------------------------ */
/** This section will include all the necessary dependence for this tsx file */
import { Button, Card, List, Typography } from "antd";
import { dataList } from "../../constant/data";
import { useNavigate } from "react-router-dom";
import { PlusOutlined } from "@ant-design/icons";
import React from 'react'
import { emailToImage } from "../../utils/emailToImage";
const { Meta } = Card;
/* <------------------------------------ **** DEPENDENCE IMPORT END **** ------------------------------------ */
/* <------------------------------------ **** INTERFACE START **** ------------------------------------ */
/** This section will include all the interface for this tsx file */
/* <------------------------------------ **** INTERFACE END **** ------------------------------------ */

/* <------------------------------------ **** FUNCTION COMPONENT START **** ------------------------------------ */
const Home = (): JSX.Element => {
  /* <------------------------------------ **** STATE START **** ------------------------------------ */
  /************* This section will include this component HOOK function *************/
  const navigate = useNavigate();
  /* <------------------------------------ **** STATE END **** ------------------------------------ */
  /* <------------------------------------ **** PARAMETER START **** ------------------------------------ */
  /************* This section will include this component parameter *************/
  /* <------------------------------------ **** PARAMETER END **** ------------------------------------ */
  /* <------------------------------------ **** FUNCTION START **** ------------------------------------ */
  /************* This section will include this component general function *************/
  const handleAdd = () => {
    navigate("/detail/-1");
  };
  
  /* <------------------------------------ **** FUNCTION END **** ------------------------------------ */
  /* <------------------------------------ **** EFFECT START **** ------------------------------------ */
  /************* This section will include this component general function *************/
  /* <------------------------------------ **** EFFECT END **** ------------------------------------ */

  return (
    <>
      <div
        style={{
          height: "calc(100vh - 70px)",
          overflow: "auto",
        }}
      >
        <List
          style={{ width: "90%", margin: "0 auto" }}
          grid={{
            gutter: 30,
            xs: 2,
            sm: 3,
            md: 4,
            lg: 5,
            xl: 6,
            xxl: 7,
          }}
          pagination={{
            pageSize: 14,
            style: { marginBottom: 24 },
          }}
          dataSource={dataList}
          renderItem={(item) => (
            <List.Item
              key={item.id}
              onClick={() => navigate(`/detail/${item.id}`)}
            >
              <Card
                styles={{ body: { padding: 10 } }}
                hoverable
                //cover={<img src={"data:image/png;base64," + (await emailToImage(item.tree))} />}
                cover={<img src={item.cover_img} />}
              >
                <Meta title={item.name} />
              </Card>
            </List.Item>
          )}
        />
      </div>
    </>
  );
};
export default Home;
/* <------------------------------------ **** FUNCTION COMPONENT END **** ------------------------------------ */
